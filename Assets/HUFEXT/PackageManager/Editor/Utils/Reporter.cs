using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Utils
{
    public struct Header
    {
        public uint Magic => magic;
        public byte Version => version;
        public ulong UploadTimestamp => uploadTimestamp;
        public byte[] Ip => ip;
        public byte[] Project => project;
        public byte[] Sku => sku;
        public byte[] PlayerId => playerId;
        public byte[] SessionId => sessionId;
        public byte[] Date => date;

        uint magic;

        byte version;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
        ulong uploadTimestamp;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        byte[] ip;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
        byte[] date;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        byte[] project;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 16 )]
        byte[] sku;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
        byte[] playerId;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
        byte[] sessionId;

        byte[] GenerateId()
        {
            return Encoding.ASCII.GetBytes( Guid.NewGuid().ToString( "N" ) );
        }

        public void Initialize( string projectName, string skuName, byte[] hufDeveloperIdHash )
        {
            magic = 0xdeadbeef;
            version = 0x1;
            project = new byte[4];
            sku = new byte[16];
            playerId = hufDeveloperIdHash;
            sessionId = GenerateId();
            date = new byte[8];
            var ipAddress = IPAddress.Parse( "0.0.0.0" );
            ip = ipAddress.GetAddressBytes();
            var shortProj = projectName.Substring( 0, Math.Min( projectName.Length, project.Length ) );
            Encoding.ASCII.GetBytes( shortProj, 0, shortProj.Length, project, 0 );
            var shortSku = skuName.Substring( 0, Math.Min( skuName.Length, sku.Length ) );
            Encoding.ASCII.GetBytes( shortSku, 0, shortSku.Length, sku, 0 );
            var dateStr = DateTime.Now.Date.ToString( "yyyyMMdd" );
            Encoding.ASCII.GetBytes( dateStr, 0, 8, date, 0 );
            uploadTimestamp = ( ulong ) DateTime.Now.ToBinary();
        }
    }

    public static class Reporter
    {
        static byte[] GetBytes( Header header, byte[] md5Checksum )
        {
            var size = Marshal.SizeOf( header );
            byte[] arr;
            using ( var ms = new MemoryStream( 125 ) )
            {
                var magic = BitConverter.GetBytes( header.Magic );
                var timestamp = BitConverter.GetBytes( header.UploadTimestamp );
                Array.Reverse( magic );
                Array.Reverse( timestamp );
                ms.Write( magic, 0, Marshal.SizeOf( header.Magic ) );
                ms.Write( BitConverter.GetBytes( header.Version ), 0, Marshal.SizeOf( header.Version ) );
                ms.Write( md5Checksum, 0, md5Checksum.Length );
                ms.Write( timestamp, 0, Marshal.SizeOf( header.UploadTimestamp ) );
                ms.Write( header.Ip, 0, header.Ip.Length );
                ms.Write( header.Date, 0, header.Date.Length );
                ms.Write( header.Project, 0, header.Project.Length );
                ms.Write( header.Sku, 0, header.Sku.Length );
                ms.Write( header.PlayerId, 0, header.PlayerId.Length );
                ms.Write( header.SessionId, 0, header.SessionId.Length );
                arr = ms.GetBuffer();
            }
            return arr;
        }

        static byte[] PackDictList( List<string> dictList )
        {
            using ( var ms = new MemoryStream() )
            {
                var serializer = MessagePackSerializer.Get<List<string>>();
                serializer.Pack( ms, dictList );
                return ms.ToArray();
            }
        }
        
        static byte[] PackReport( Dictionary<long, long> report, long timestamp )
        {
            using ( var ms = new MemoryStream() )
            {
                var reportEvent = new Tuple<int, long, Dictionary<long, long>>( 1, timestamp, report );
                var pack = new Tuple<Tuple<int, long, Dictionary<long, long>>>(reportEvent);
                var serializer = MessagePackSerializer.Get<Tuple<Tuple<int, long, Dictionary<long, long>>>>();
                serializer.Pack( ms, pack );
                return ms.ToArray();
            }
        }

        static byte[] HashChecksum( byte[] bytesToHash )
        {
            using ( var hash = MD5.Create() )
            {
                var generatedHash = hash.ComputeHash( bytesToHash );
                return generatedHash;
            }
        }

        static Tuple<List<string>, Dictionary<long, long>> CreateDictAndParsedReport( Dictionary<string, string> report )
        {
            var dictMap = new Dictionary<string, int>();
            var newReport = new Dictionary<long, long>();
            var dictList = new List<string>();
            foreach ( var pair in report )
            {
                if( !dictMap.ContainsKey( pair.Key ) )
                {
                    dictMap.Add( pair.Key, dictMap.Count );
                }

                var newKey = ( long ) dictMap[pair.Key];
                newKey <<= 1;
                newKey |= 1;

                if ( !dictMap.ContainsKey( pair.Value ) )
                {
                    dictMap.Add( pair.Value, dictMap.Count );
                }

                var newVal = ( long ) dictMap[pair.Value];

                newReport.Add( newKey, newVal );
            }

            var sortedDictEnumerable = dictMap.OrderBy( x => x.Value );
            foreach ( var pair in sortedDictEnumerable )
            {
                dictList.Add( pair.Key );
            }
            
            return new Tuple<List<string>, Dictionary<long,long>>( dictList , newReport );
        }

        static byte[] PackBytes( byte[] firstBytes, byte[] secondBytes )
        {
            var packageBytes = new byte[firstBytes.Length + secondBytes.Length];
            var dstOffset = 0;
            var count = firstBytes.Length;
            Buffer.BlockCopy( firstBytes, 0, packageBytes, dstOffset, count );
            dstOffset += count;
            count = secondBytes.Length;
            Buffer.BlockCopy( secondBytes, 0, packageBytes, dstOffset, count );
            return packageBytes;
        }

        static void Upload( byte[] dataBytes )
        {
            var uriBuilder = new UriBuilder( Models.Keys.REPORT_URL );
            var request = WebRequest.Create( uriBuilder.Uri );
            request.Method = "POST";
            request.ContentLength = dataBytes.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write( dataBytes, 0, dataBytes.Length );
            dataStream.Close();
            
            var response = request.GetResponse();
#if HPM_DEV_MODE
            Debug.Log(
                $"Report: {response.ResponseUri} with result: {( ( HttpWebResponse ) response ).StatusDescription}" );
#endif
            response.Close();
        }

        public static void Send( Dictionary<string, string> report )
        {
            var header = new Header();
            var devIdHash = HashChecksum( Encoding.ASCII.GetBytes( report[Models.Keys.BuildEventKey.DEV_ID] ) );
            header.Initialize( "huf", "report", PackBytes( devIdHash, devIdHash ) );
            CreateDictAndParsedReport( report ).Deconstruct( out var dictList, out var reportMap );
            var dictListBytes = PackDictList( dictList );
            var timestamp = DateTime.ParseExact( report[Models.Keys.BuildEventKey.BUILD_TIME],
                                        "MM/dd/yyyy HH:mm:ss",
                                        CultureInfo.InvariantCulture )
                                    .ToBinary();
            var reportBytes = PackReport( reportMap, timestamp );
            var dictAndReportBytes = PackBytes( dictListBytes, reportBytes );
            var headerBytes = GetBytes( header, HashChecksum( dictAndReportBytes ) );
            Task.Run( () => Upload( PackBytes( headerBytes, dictAndReportBytes ) ) );
        }
    }
}

// Wrapper for backward compatibility.
namespace HUF.Build.Report
{
    public static class Sender
    {
        [Obsolete( "Use Utils.Reporter.Send() instead." )]
        public static void SendReport( Dictionary<string, string> report ) =>
            HUFEXT.PackageManager.Editor.Utils.Reporter.Send( report );
    }
}