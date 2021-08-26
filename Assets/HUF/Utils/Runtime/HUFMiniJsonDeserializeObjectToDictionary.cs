using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public static partial class HUFJson
    {
        const char COLON = ':';

        public static Dictionary<string, string> DeserializeObjectToDictionary( string json )
        {
            if ( json == null )
                return new Dictionary<string, string>();

            var outcome = HUFJson.Parser.DeserializeObjectToDictionary( json );
            return outcome;
        }

        private partial class Parser : IDisposable
        {
            public static Dictionary<string, string> DeserializeObjectToDictionary( string jsonString )
            {
                using ( HUFJson.Parser parser = new HUFJson.Parser( jsonString ) )
                    return parser.DeserializeObjectToDictionary();
            }

            Dictionary<string, string> DeserializeObjectToDictionary()
            {
                var dictionary = new Dictionary<string, string>();
                int curlyDepth = 0;
                int squaredDepth = 0;
                int lastPartIndex = 1;
                int index = 0;
                bool isEscaped = false;
                bool isWord = false;
                string currentPropertyName = "";
                var stringBuilder = new StringBuilder();

                void ProcessToken()
                {
                    char peekChar = PeekChar;

                    switch ( peekChar )
                    {
                        case CURLY_OPEN when !isWord && !isEscaped:
                            curlyDepth++;
                            break;
                        case CURLY_CLOSE when !isWord && !isEscaped:
                            curlyDepth--;
                            break;
                        case SQUARED_OPEN when !isWord && !isEscaped:
                            squaredDepth++;
                            break;
                        case SQUARED_CLOSE when !isWord && !isEscaped:
                            squaredDepth--;
                            break;
                        case COMMA when !isWord && !isEscaped:
                            if ( curlyDepth == 1 && squaredDepth == 0 )
                            {
                                AddToDictionary();
                            }

                            break;
                        case COLON when !isWord && !isEscaped:
                            if ( curlyDepth == 1 && squaredDepth == 0 )
                                lastPartIndex = index + 1;
                            break;
                        case QUOTE when !isEscaped:
                            isWord = !isWord;

                            if ( curlyDepth == 1 && squaredDepth == 0 && !isWord )
                            {
                                currentPropertyName = stringBuilder.ToString();
                                stringBuilder.Clear();
                            }

                            break;
                        default:
                            if ( peekChar == ESCAPE )
                                isEscaped = !isEscaped;

                            if ( curlyDepth == 1 && squaredDepth == 0 && isWord )
                            {
                                stringBuilder.Append( peekChar );
                            }

                            break;
                    }

                    if ( isEscaped && peekChar != ESCAPE )
                        isEscaped = false;
                }

                void AddToDictionary()
                {
                    if ( lastPartIndex != index )
                        dictionary.Add( currentPropertyName,
                            jsonString.Substring( lastPartIndex, index - lastPartIndex ).Trim() );
                    lastPartIndex = index + 1;
                }

                do
                {
                    ProcessToken();
                    json.Read();
                    index++;
                } while ( index < jsonString.Length && curlyDepth > 0 );

                if ( currentPropertyName.Length > 0 )
                    AddToDictionary();
                return dictionary;
            }
        }
    }
}