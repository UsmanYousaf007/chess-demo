using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using HUF.Utils.Runtime.AndroidManifest;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEditor;

namespace HUF.Utils.Editor.AndroidManifest
{
    public static class AndroidManifestKeyReplacer
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AndroidManifestKeyReplacer) );
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public static void CreateFinalManifest( FeatureConfigBase config, string manifestTemplatePath, string savePath )
        {
            var xmlDocument = ReadXmlFile( manifestTemplatePath );

            if ( xmlDocument == null )
                return;

            var type = config.GetType();
            var fields = type.GetFields( flags );
            var properties = type.GetProperties( flags );
            var fieldsAndProperties = fields.Cast<MemberInfo>().Concat( properties );

            foreach ( var field in fieldsAndProperties )
            {
                var attribute =
                    field.GetCustomAttribute( typeof(AndroidManifestAttribute), false ) as AndroidManifestAttribute;

                if ( attribute != null )
                {
                    var key = field is FieldInfo
                        ? (string)( field as FieldInfo ).GetValue( config )
                        : (string)( field as PropertyInfo )?.GetValue( config );
                    var nodes = xmlDocument.GetElementsByTagName( attribute.Tag );
                    var matchingNodes = FindNodesWithAttribute( nodes, attribute );

                    foreach ( var node in matchingNodes )
                    {
                        if ( key.IsNullOrEmpty() )
                        {
                            node.ParentNode?.RemoveChild( node );
                        }
                        else
                        {
                            ReplaceAttributeValue( node, attribute, key );
                        }
                    }
                }

                var permissionsAttribute =
                    field.GetCustomAttribute( typeof(AndroidManifestAddPermissions), false ) as
                        AndroidManifestAddPermissions;

                if ( permissionsAttribute != null )
                {
                    var permissions = field is FieldInfo
                        ? (string[])( field as FieldInfo ).GetValue( config )
                        : (string[])( field as PropertyInfo )?.GetValue( config );
                    var manifestNode = xmlDocument.LastChild;
                    var permissionNode = manifestNode.LastChild;

                    if ( permissions == null || permissions.Length == 0 )
                    {
                        manifestNode.RemoveChild( permissionNode );
                        continue;
                    }

                    for ( int i = 0; i < permissions.Length; i++ )
                    {
                        if ( i == 0 )
                            permissionNode.Attributes[0].Value = $"android.permission.{permissions[i]}";
                        else
                        {
                            var node = permissionNode.CloneNode( true );
                            node.Attributes[0].Value = $"android.permission.{permissions[i]}";
                            manifestNode.AppendChild( node );
                        }
                    }
                }
            }

            WriteXmlToFile( savePath, xmlDocument );
        }

        static void ReplaceAttributeValue( XmlNode node, AndroidManifestAttribute attribute, string key )
        {
            foreach ( XmlAttribute attr in node.Attributes )
            {
                if ( attr.Name == attribute.Attribute &&
                     attr.Value == attribute.ValueToReplace )
                {
                    attr.Value = key;
                }
            }
        }

        static IEnumerable<XmlNode> FindNodesWithAttribute( XmlNodeList nodes, AndroidManifestAttribute attribute )
        {
            var matchingNodes = new List<XmlNode>();

            foreach ( XmlNode node in nodes )
            {
                if ( node.Attributes == null )
                {
                    continue;
                }

                foreach ( XmlAttribute nodeAttribute in node.Attributes )
                {
                    if ( nodeAttribute.Name == attribute.Attribute &&
                         nodeAttribute.Value == attribute.ValueToReplace )
                    {
                        matchingNodes.Add( node );
                    }
                }
            }

            return matchingNodes;
        }

        static XmlDocument ReadXmlFile( string path )
        {
            var xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.Load( path );
            }
            catch ( Exception )
            {
                HLog.LogWarning( logPrefix, $"Manifest template for {path} not found." );
            }

            return xmlDocument;
        }

        static void WriteXmlToFile( string path, XmlDocument xmlDocument )
        {
            var finalPath = path;
            var finalFolderPath = path;

            if ( path.Contains( ".xml" ) )
            {
                var strippedPath = path
                    .Split( '/' )
                    .Where( x => !x.Contains( ".xml" ) )
                    .ToList();
                finalFolderPath = Path.Combine( strippedPath.ToArray() );
                strippedPath.Add( "AndroidManifest.xml" );
                finalPath = Path.Combine( strippedPath.ToArray() );
            }

            if ( !Directory.Exists( finalFolderPath ) )
                Directory.CreateDirectory( finalFolderPath );
            xmlDocument.Save( finalPath );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
        }
    }
}