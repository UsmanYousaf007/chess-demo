using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;

namespace HUFEXT.ModuleStarter.Runtime.Config
{
    public static class ModuleInitializerConfig
    {
        const string FILENAME = "initOrder.dat";

        static Dictionary<string, OrderEntry> entries;
        static List<OrderEntry> sortedEntries;
        static int newPlaces;
        public static IEnumerable<OrderEntry> Entries => sortedEntries;

        static readonly string basePath = Application.streamingAssetsPath;
        static readonly string path = Path.Combine( basePath, FILENAME );

        static ModuleInitializerConfig()
        {
            Load();
        }

        public static int ProcessOrder( string name )
        {
            if ( entries.TryGetValue( name, out OrderEntry entry ) )
                return entry.order;

            newPlaces++;
            OrderEntry newEntry = new OrderEntry()
            {
                id = name,
                order = -newPlaces
            };

            entries.Add( name, newEntry );

            Save();

            return newEntry.order;
        }

        public static void SortEntries()
        {
            sortedEntries = entries.Values.ToList();
            sortedEntries.Sort( OrderEntry.Compare );
        }

        public static void Load()
        {
            string[] lines = SynchronousLoad();
            entries = new Dictionary<string, OrderEntry>( lines.Length );

            foreach ( var line in lines )
            {
                if ( line.IsNullOrEmpty() )
                    continue;

                var entry = JsonUtility.FromJson<OrderEntry>( line );
                entries.Add( entry.id, entry );
            }
        }

        static void Save()
        {
#if UNITY_EDITOR
            if ( !Directory.Exists( basePath ) )
                Directory.CreateDirectory( basePath );

            using ( File.Open( path, FileMode.OpenOrCreate ) ) { }

            string[] lines = new string[entries.Count];

            int index = 0;
            foreach ( var entry in entries.Values )
            {
                lines[index++] = JsonUtility.ToJson( entry );
            }
            File.WriteAllLines( path, lines );
#endif
        }

        public static void UpdateEntries( List<OrderEntry> orderedList )
        {
            int length = orderedList.Count;
            for ( int index = 0; index < length; index++ )
            {
                OrderEntry orderEntry = orderedList[index];
                var entry = entries[orderEntry.id];
                entry.order = index;
                entry.isAsync = orderEntry.isAsync;
                entry.isSkipped = orderEntry.isSkipped;
            }
            Save();
        }

        static string[] SynchronousLoad()
        {
#if UNITY_EDITOR || !UNITY_ANDROID
            string streamingPath = $"file://{path}";
#else
            string streamingPath = path;
#endif

            WWW data = new WWW( streamingPath );

            while ( data.MoveNext() ) { }

            if ( string.IsNullOrEmpty( data.text ) )
                return new string[0];

            return data.text.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.RemoveEmptyEntries
            );
        }

        [Serializable]
        public class OrderEntry
        {
            public string id;
            public int order;
            public bool isAsync = true;
            public bool isSkipped;

            public static int Compare( OrderEntry a, OrderEntry b )
            {
                if ( a.order == b.order )
                    return 0;

                return a.order < b.order ? -1 : 1;
            }
        }
    }
}