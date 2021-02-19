using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public static class HUFJson
    {
        public static string SerializeStringDictionary( Dictionary<string, string> dictionary )
        {
            var toJson = new DictArrayHelper( dictionary.Count );
            var index = 0;

            foreach ( var item in dictionary )
            {
                toJson.items[index] = new DictHelper {key = item.Key, value = item.Value};
                index++;
            }

            return JsonUtility.ToJson( toJson );
        }

        public static string SerializeStringDictionary<T>( Dictionary<string, T> dictionary, Func<T, string> serializer )
        {
            var toJson = new DictArrayHelper( dictionary.Count );
            var index = 0;

            foreach ( var item in dictionary )
            {
                toJson.items[index] = new DictHelper {key = item.Key, value = serializer( item.Value )};
                index++;
            }

            return JsonUtility.ToJson( toJson );
        }

        public static Dictionary<string, string> DeserializeStringDictionary( string json )
        {
            DictArrayHelper fromJson = JsonUtility.FromJson<DictArrayHelper>( json );
            int length = fromJson.items.Length;
            var dictionary = new Dictionary<string, string>( length );

            for ( var i = 0; i < length; i++ )
            {
                dictionary[fromJson.items[i].key] = fromJson.items[i].value;
            }

            return dictionary;
        }

        public static Dictionary<string, T> DeserializeStringDictionary<T>( string json, Func<string,T> parser )
        {
            DictArrayHelper fromJson = JsonUtility.FromJson<DictArrayHelper>( json );
            int length = fromJson.items.Length;
            var dictionary = new Dictionary<string, T>( length );

            for ( var i = 0; i < length; i++ )
            {
                dictionary[fromJson.items[i].key] = parser( fromJson.items[i].value );
            }

            return dictionary;
        }

        public static List<T> DeserializeList<T>( string json )
        {
            if ( json.IsNullOrEmpty() || json == "[]" )
                return new List<T>();

            return ListParser<T>.Parse( json );
        }

        public static object Deserialize( string json )
        {
            if ( json == null )
                return (object)null;

            object outcome = HUFJson.Parser.Parse( json );

            if ( outcome is ISerializationCallbackReceiver receiver )
                receiver.OnAfterDeserialize();

            return outcome;
        }

        public static string Serialize( object obj )
        {
            if ( obj is ISerializationCallbackReceiver receiver )
                receiver.OnBeforeSerialize();

            return HUFJson.Serializer.Serialize( obj );
        }

        [Serializable]
        struct DictHelper
        {
            public string key;
            public string value;
        }

        [Serializable]
        struct DictArrayHelper
        {
            public DictHelper[] items;

            public DictArrayHelper( int length )
            {
                items = new DictHelper[length];
            }
        }

        static class ListParser<T>
        {
            public static List<T> Parse( string jsonList )
            {
                string data = $"{{\"{nameof(ListWrapper.list)}\":{jsonList}}}";
                var wrapped = JsonUtility.FromJson<ListWrapper>( data );
                return wrapped.list;
            }

            [Serializable]
            struct ListWrapper
            {
#pragma warning disable 649
                public List<T> list;
#pragma warning restore 649
            }
        }

        private sealed class Parser : IDisposable
        {
            private const string WORD_BREAK = "{}[],:\"";
            private StringReader json;

            private Parser( string jsonString )
            {
                this.json = new StringReader( jsonString );
            }

            public static bool IsWordBreak( char c )
            {
                if ( !char.IsWhiteSpace( c ) )
                    return "{}[],:\"".IndexOf( c ) != -1;

                return true;
            }

            public static object Parse( string jsonString )
            {
                using ( HUFJson.Parser parser = new HUFJson.Parser( jsonString ) )
                    return parser.ParseValue();
            }

            public void Dispose()
            {
                this.json.Dispose();
                this.json = (StringReader)null;
            }

            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                this.json.Read();

                while ( true )
                {
                    HUFJson.Parser.TOKEN nextToken;

                    do
                    {
                        nextToken = this.NextToken;

                        switch ( nextToken )
                        {
                            case HUFJson.Parser.TOKEN.NONE:
                                goto label_3;
                            case HUFJson.Parser.TOKEN.CURLY_CLOSE:
                                goto label_4;
                            default:
                                continue;
                        }
                    } while ( nextToken == HUFJson.Parser.TOKEN.COMMA );

                    string index = this.ParseString();

                    if ( index != null )
                    {
                        if ( this.NextToken == HUFJson.Parser.TOKEN.COLON )
                        {
                            this.json.Read();
                            dictionary[index] = this.ParseValue();
                        }
                        else
                            goto label_8;
                    }
                    else
                        goto label_6;
                }

                label_3:
                return (Dictionary<string, object>)null;

                label_4:
                return dictionary;

                label_6:
                return (Dictionary<string, object>)null;

                label_8:
                return (Dictionary<string, object>)null;
            }

            private List<object> ParseArray()
            {
                List<object> objectList = new List<object>();
                this.json.Read();
                bool flag = true;

                while ( flag )
                {
                    HUFJson.Parser.TOKEN nextToken = this.NextToken;

                    switch ( nextToken )
                    {
                        case HUFJson.Parser.TOKEN.SQUARED_CLOSE:
                            flag = false;
                            continue;
                        case HUFJson.Parser.TOKEN.COMMA:
                            continue;
                        default:
                            if ( nextToken == HUFJson.Parser.TOKEN.NONE )
                                return (List<object>)null;

                            object byToken = this.ParseByToken( nextToken );
                            objectList.Add( byToken );
                            continue;
                    }
                }

                return objectList;
            }

            private object ParseValue()
            {
                return this.ParseByToken( this.NextToken );
            }

            private object ParseByToken( HUFJson.Parser.TOKEN token )
            {
                switch ( token )
                {
                    case HUFJson.Parser.TOKEN.CURLY_OPEN:
                        return (object)this.ParseObject();
                    case HUFJson.Parser.TOKEN.SQUARED_OPEN:
                        return (object)this.ParseArray();
                    case HUFJson.Parser.TOKEN.STRING:
                        return (object)this.ParseString();
                    case HUFJson.Parser.TOKEN.NUMBER:
                        return this.ParseNumber();
                    case HUFJson.Parser.TOKEN.TRUE:
                        return (object)true;
                    case HUFJson.Parser.TOKEN.FALSE:
                        return (object)false;
                    case HUFJson.Parser.TOKEN.NULL:
                        return (object)null;
                    default:
                        return (object)null;
                }
            }

            private string ParseString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                this.json.Read();
                bool flag = true;

                while ( flag )
                {
                    if ( this.json.Peek() == -1 )
                        break;

                    char nextChar1 = this.NextChar;

                    switch ( nextChar1 )
                    {
                        case '"':
                            flag = false;
                            continue;
                        case '\\':
                            if ( this.json.Peek() == -1 )
                            {
                                flag = false;
                                continue;
                            }

                            char nextChar2 = this.NextChar;

                            switch ( nextChar2 )
                            {
                                case 'r':
                                    stringBuilder.Append( '\r' );
                                    continue;
                                case 't':
                                    stringBuilder.Append( '\t' );
                                    continue;
                                case 'u':
                                    char[] chArray = new char[4];

                                    for ( int index = 0; index < 4; ++index )
                                        chArray[index] = this.NextChar;
                                    stringBuilder.Append( (char)Convert.ToInt32( new string( chArray ), 16 ) );
                                    continue;
                                default:
                                    if ( nextChar2 != '"' && nextChar2 != '/' && nextChar2 != '\\' )
                                    {
                                        switch ( nextChar2 )
                                        {
                                            case 'b':
                                                stringBuilder.Append( '\b' );
                                                continue;
                                            case 'f':
                                                stringBuilder.Append( '\f' );
                                                continue;
                                            case 'n':
                                                stringBuilder.Append( '\n' );
                                                continue;
                                            default:
                                                continue;
                                        }
                                    }
                                    else
                                    {
                                        stringBuilder.Append( nextChar2 );
                                        continue;
                                    }
                            }
                        default:
                            stringBuilder.Append( nextChar1 );
                            continue;
                    }
                }

                return stringBuilder.ToString();
            }

            private object ParseNumber()
            {
                string nextWord = this.NextWord;

                if ( nextWord.IndexOf( '.' ) == -1 && nextWord.IndexOf( 'E' ) == -1 && nextWord.IndexOf( 'e' ) == -1 )
                {
                    long result;

                    long.TryParse( nextWord,
                        NumberStyles.Any,
                        (IFormatProvider)CultureInfo.InvariantCulture,
                        out result );
                    return (object)result;
                }

                double result1;

                double.TryParse( nextWord,
                    NumberStyles.Any,
                    (IFormatProvider)CultureInfo.InvariantCulture,
                    out result1 );
                return (object)result1;
            }

            private void EatWhitespace()
            {
                while ( char.IsWhiteSpace( this.PeekChar ) )
                {
                    this.json.Read();

                    if ( this.json.Peek() == -1 )
                        break;
                }
            }

            private char PeekChar
            {
                get { return Convert.ToChar( this.json.Peek() ); }
            }

            private char NextChar
            {
                get { return Convert.ToChar( this.json.Read() ); }
            }

            private string NextWord
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    while ( !HUFJson.Parser.IsWordBreak( this.PeekChar ) )
                    {
                        stringBuilder.Append( this.NextChar );

                        if ( this.json.Peek() == -1 )
                            break;
                    }

                    return stringBuilder.ToString();
                }
            }

            private HUFJson.Parser.TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();

                    if ( this.json.Peek() == -1 )
                        return HUFJson.Parser.TOKEN.NONE;

                    char peekChar = this.PeekChar;

                    switch ( peekChar )
                    {
                        case ',':
                            this.json.Read();
                            return HUFJson.Parser.TOKEN.COMMA;
                        case '-':
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            return HUFJson.Parser.TOKEN.NUMBER;
                        case ':':
                            return HUFJson.Parser.TOKEN.COLON;
                        default:
                            switch ( peekChar )
                            {
                                case '[':
                                    return HUFJson.Parser.TOKEN.SQUARED_OPEN;
                                case ']':
                                    this.json.Read();
                                    return HUFJson.Parser.TOKEN.SQUARED_CLOSE;
                                default:
                                    switch ( peekChar )
                                    {
                                        case '{':
                                            return HUFJson.Parser.TOKEN.CURLY_OPEN;
                                        case '}':
                                            this.json.Read();
                                            return HUFJson.Parser.TOKEN.CURLY_CLOSE;
                                        default:
                                            if ( peekChar == '"' )
                                                return HUFJson.Parser.TOKEN.STRING;

                                            switch ( this.NextWord )
                                            {
                                                case "false":
                                                    return HUFJson.Parser.TOKEN.FALSE;
                                                case "true":
                                                    return HUFJson.Parser.TOKEN.TRUE;
                                                case "null":
                                                    return HUFJson.Parser.TOKEN.NULL;
                                                default:
                                                    return HUFJson.Parser.TOKEN.NONE;
                                            }
                                    }
                            }
                    }
                }
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL,
            }
        }

        private sealed class Serializer
        {
            private StringBuilder builder;

            private Serializer()
            {
                this.builder = new StringBuilder();
            }

            public static string Serialize( object obj )
            {
                HUFJson.Serializer serializer = new HUFJson.Serializer();
                serializer.SerializeValue( obj );
                return serializer.builder.ToString();
            }

            private void SerializeValue( object value )
            {
                if ( value == null )
                    this.builder.Append( "null" );
                else if ( value is string str )
                    this.SerializeString( str );
                else if ( value is bool )
                    this.builder.Append( !(bool)value ? "false" : "true" );
                else if ( value is IList anArray )
                    this.SerializeArray( anArray );
                else if ( value is IDictionary dictionary )
                    this.SerializeObject( dictionary );
                else if ( value is char )
                    this.SerializeString( new string( (char)value, 1 ) );
                else
                    this.SerializeOther( value );
            }

            private void SerializeObject( IDictionary obj )
            {
                bool flag = true;
                this.builder.Append( '{' );
                IEnumerator enumerator = obj.Keys.GetEnumerator();

                try
                {
                    while ( enumerator.MoveNext() )
                    {
                        object current = enumerator.Current;

                        if ( !flag )
                            this.builder.Append( ',' );
                        this.SerializeString( current.ToString() );
                        this.builder.Append( ':' );
                        this.SerializeValue( obj[current] );
                        flag = false;
                    }
                }
                finally
                {
                    if ( enumerator is IDisposable disposable )
                        disposable.Dispose();
                }

                this.builder.Append( '}' );
            }

            private void SerializeArray( IList anArray )
            {
                this.builder.Append( '[' );
                bool flag = true;

                for ( int index = 0; index < anArray.Count; ++index )
                {
                    object an = anArray[index];

                    if ( !flag )
                        this.builder.Append( ',' );
                    this.SerializeValue( an );
                    flag = false;
                }

                this.builder.Append( ']' );
            }

            private void SerializeString( string str )
            {
                this.builder.Append( '"' );

                foreach ( char ch in str.ToCharArray() )
                {
                    switch ( ch )
                    {
                        case '\b':
                            this.builder.Append( "\\b" );
                            break;
                        case '\t':
                            this.builder.Append( "\\t" );
                            break;
                        case '\n':
                            this.builder.Append( "\\n" );
                            break;
                        case '\f':
                            this.builder.Append( "\\f" );
                            break;
                        case '\r':
                            this.builder.Append( "\\r" );
                            break;
                        default:
                            switch ( ch )
                            {
                                case '"':
                                    this.builder.Append( "\\\"" );
                                    continue;
                                case '\\':
                                    this.builder.Append( "\\\\" );
                                    continue;
                                default:
                                    int int32 = Convert.ToInt32( ch );

                                    if ( int32 >= 32 && int32 <= 126 )
                                    {
                                        this.builder.Append( ch );
                                        continue;
                                    }

                                    this.builder.Append( "\\u" );
                                    this.builder.Append( int32.ToString( "x4" ) );
                                    continue;
                            }
                    }
                }

                this.builder.Append( '"' );
            }

            private void SerializeOther( object value )
            {
                if ( value is float )
                    this.builder.Append(
                        ( (float)value ).ToString( "R", (IFormatProvider)CultureInfo.InvariantCulture ) );
                else if ( value is int || value is uint || ( value is long || value is sbyte ) ||
                          ( value is byte || value is short || ( value is ushort || value is ulong ) ) )
                    this.builder.Append( value );
                else if ( value is double || value is Decimal )
                    this.builder.Append( Convert.ToDouble( value )
                        .ToString( "R", (IFormatProvider)CultureInfo.InvariantCulture ) );
                else
                    this.SerializeString( value.ToString() );
            }
        }
    }
}