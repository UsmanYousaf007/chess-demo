using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.Utils.Editor.BuildSupport
{
    [UsedImplicitly]
    public class DefineCollector : IPreprocessBuildWithReport
    {
        const string FORMATTER_COMMENT = "#\"{0}\"";
        const string FORMATTER_HEADER = "#\"\"\"\"\"\"\"{0}\"\"\"\"\"\"\"";
        const string HEADER_HUF = "HUF DEFINES";
        const string HEADER_CUSTOM = "DEV DEFINES";
        const char HEADER_FILL = '"';
        const string PATH_HUF_UNIX = "Assets/HUF";
        const string PATH_HUF_WIN = "Assets\\HUF";
        const string PATH_HUFEXT_UNIX = "Assets/HUFEXT";
        const string PATH_HUFEXT_WIN = "Assets\\HUFEXT";
        const string PATTERN_HUFDEFINE = "define.hufdefine";
        const string PATTERN_DEFINE_ENTRY = "-define:";
        const int PADDING = 45;

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(DefineCollector) );

        static readonly Regex defineValidator = new Regex( @"^\w*$" );

        static readonly string[] rspFiles =
        {
            "mcs.rsp",
            "csc.rsp"
        };

        public int callbackOrder => int.MinValue;

        [MenuItem("HUF/Utils/Rebuild Defines")]
        [PublicAPI]
        public static void RebuildDefines()
        {
            GatherDefines( out DefineDefinition[] hufDefines, out DefineDefinition[] customDefines );

            AppendDefines( hufDefines, HEADER_HUF );
            AppendDefines( customDefines, HEADER_CUSTOM );

            HLog.LogImportant( logPrefix, $"Processed {hufDefines.Length + customDefines.Length} define symbols" );

            AssetDatabase.Refresh();
        }

        public void OnPreprocessBuild( BuildReport report )
        {
            RebuildDefines();
        }

        static void GatherDefines(out DefineDefinition[] hufDefines, out DefineDefinition[] customDefines)
        {
            string[] files = GetFiles( PATTERN_HUFDEFINE );
            HLog.Log( logPrefix, $"Found {files.Length} define files" );

            var hufDefinitions = new List<DefineDefinition>( files.Length );
            var customDefinitions = new List<DefineDefinition>();

            for ( int fileIndex = 0; fileIndex < files.Length; fileIndex++ )
            {
                var file = files[fileIndex];

                string[] defines = DefineParser.Parse( File.ReadAllText( file ), file );

                bool isHuf = file.Contains( PATH_HUF_UNIX )
                             || file.Contains( PATH_HUFEXT_UNIX )
                             || file.Contains( PATH_HUF_WIN )
                             || file.Contains( PATH_HUFEXT_WIN );

                List<DefineDefinition> container = isHuf ? hufDefinitions : customDefinitions;
                string relativePath = file.Substring( Application.dataPath.Length );
                relativePath = relativePath.Substring( 0, relativePath.Length - PATTERN_HUFDEFINE.Length - 1);

                for ( int i = 0; i < defines.Length; i++ )
                {
                    container.Add( new DefineDefinition()
                    {
                        define = defines[i],
                        path = $"Assets{relativePath}"
                    } );
                }
            }

            hufDefines = hufDefinitions.ToArray();
            customDefines = customDefinitions.ToArray();
        }

        static void AppendDefines( DefineDefinition[] defines, string header )
        {
            string headerLine = string.Format( FORMATTER_HEADER, header );
            string endLine = string.Format( FORMATTER_HEADER, new String( HEADER_FILL, header.Length ) );

            foreach ( string filename in rspFiles )
            {
                string path = Path.Combine( Application.dataPath, filename );
                using ( File.Open( path, FileMode.OpenOrCreate ) ) { }

                string[] lines = File.ReadAllLines( path );

                bool hasHUFSection = false;
                bool isHUFSection = false;

                List<string> newLines = new List<string>( lines.Length );

                for ( int i = 0; i < lines.Length; i++ )
                {
                    string line = lines[i];

                    if ( isHUFSection )
                    {
                        if ( line != endLine )
                            continue;

                        AddDefineLines( newLines, defines );
                        isHUFSection = false;
                    }
                    else if ( line == headerLine )
                    {
                        hasHUFSection = true;
                        isHUFSection = true;
                    }

                    newLines.Add( line );
                }

                if ( !hasHUFSection )
                {
                    newLines.Add( headerLine );
                    AddDefineLines( newLines, defines );
                    newLines.Add( endLine );
                }

                File.WriteAllLines( path, newLines );
            }
        }

        static void AddDefineLines( List<string> lines, DefineDefinition[] defines )
        {
            for ( int i = 0; i < defines.Length; i++ )
            {
                lines.Add( defines[i].ToString() );
            }
        }

        static string[] GetFiles( string pattern )
        {
            return Directory.GetFiles( Application.dataPath, pattern, SearchOption.AllDirectories );
        }

        struct DefineDefinition
        {
            public string define;
            public string path;

            static readonly int paddingBase;

            static DefineDefinition()
            {
                paddingBase = PADDING - PATTERN_DEFINE_ENTRY.Length;
            }

            public override string ToString()
            {
                string universalPath = path.Replace( '\\', '/' );
                int fillCount = paddingBase - define.Length;
                string padding = new string( ' ', fillCount > 0 ? fillCount : 0 );
                return $"{PATTERN_DEFINE_ENTRY}{define}{padding}{string.Format( FORMATTER_COMMENT, $"from: {universalPath}" )}";
            }
        }

        static class DefineParser
        {
            const string COMMENT = "#";

            public static string[] Parse( string source, string path )
            {
                string[] lines = source.Split( new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries );
                List<string> defines = new List<string>( 2 );

                for ( int i = 0; i < lines.Length; i++ )
                {
                    if(lines[i].StartsWith( COMMENT ))
                        continue;

                    if ( !defineValidator.IsMatch( lines[i] ) )
                    {
                        HLog.LogError( logPrefix, $"Incorrect define \"{lines[i]}\" in {path}. Skipping" );
                        continue;
                    }

                    defines.Add( lines[i] );
                }

                return defines.ToArray();
            }
        }
    }
}