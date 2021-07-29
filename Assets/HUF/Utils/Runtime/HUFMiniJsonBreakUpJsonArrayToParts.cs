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
        const char CURLY_OPEN = '{';
        const char CURLY_CLOSE = '}';
        const char SQUARED_OPEN = '[';
        const char SQUARED_CLOSE = ']';
        const char QUOTE = '"';
        const char ESCAPE = '\\';
        const char COMMA = ',';

        public static List<string> BreakUpJsonArrayToParts( string json )
        {
            return Parser.BreakUpJsonArrayToParts( json );
        }

        private partial class Parser : IDisposable
        {
            public static List<string> BreakUpJsonArrayToParts( string jsonString )
            {
                using ( HUFJson.Parser parser = new HUFJson.Parser( jsonString ) )
                    return parser.BreakUpJsonArrayToParts();
            }

            List<string> BreakUpJsonArrayToParts()
            {
                var parts = new List<string>();
                int curlyDepth = 0;
                int squaredDepth = 0;
                int lastPartIndex = 1;
                int index = 0;
                bool isEscaped = false;
                bool isWord = false;

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

                            if ( squaredDepth == 0 )
                                AddJsonPart();
                            break;
                        case COMMA when !isWord && !isEscaped:
                            if ( squaredDepth == 1 && curlyDepth == 0 )
                            {
                                AddJsonPart();
                            }

                            break;
                        case QUOTE when !isEscaped:
                            isWord = !isWord;
                            break;
                        case ESCAPE:
                            isEscaped = !isEscaped;
                            break;
                    }

                    if ( isEscaped && peekChar != ESCAPE )
                        isEscaped = false;
                }

                void AddJsonPart()
                {
                    if ( lastPartIndex != index )
                        parts.Add( jsonString.Substring( lastPartIndex, index - lastPartIndex ).Trim() );
                    lastPartIndex = index + 1;
                }

                do
                {
                    ProcessToken();
                    json.Read();
                    index++;
                } while ( index < jsonString.Length && squaredDepth > 0 );

                return parts;
            }
        }
    }
}