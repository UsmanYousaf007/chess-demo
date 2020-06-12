using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Utils.Runtime.Performance
{
    public static class StopwatchManager
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( $"Utils_{nameof(StopwatchManager)}" );
        static Dictionary<string, Stopwatch> stopwatches;
        static Dictionary<string, long> stopwatchesFromStart;

        static Stopwatch gameStartTime;

        const float MILLISECONDS_IN_SECOND = 1000f;

#if UNITY_2019_1_OR_NEWER
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
#else
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
#endif
        static void Init()
        {
            if ( gameStartTime == null )
            {
                gameStartTime = Stopwatch.StartNew();
            }
        }

        public static void StartStopwatch( string label )
        {
            if ( stopwatches == null )
            {
                stopwatches = new Dictionary<string, Stopwatch>();
            }

            if ( stopwatches.ContainsKey( label ) )
            {
                HLog.LogWarning( logPrefix, $"Stopwatch with following label already exists: {label}" );
                return;
            }

            stopwatches.Add( label, Stopwatch.StartNew() );
        }

        public static void StopStopwatchFromStartTime( string label )
        {
            if ( stopwatchesFromStart == null )
            {
                stopwatchesFromStart = new Dictionary<string, long>();
            }

            if ( gameStartTime == null )
            {
                gameStartTime = Stopwatch.StartNew();
            }

            if ( stopwatchesFromStart.ContainsKey( label ) )
            {
                HLog.LogWarning( logPrefix, $"Stopwatch with following label already exists: {label}" );
                return;
            }

            stopwatchesFromStart.Add( label, gameStartTime.ElapsedMilliseconds );
            PrintStopWatchInSeconds( label );
        }

        public static void StopStopwatch( string label )
        {
            if ( stopwatches == null || !stopwatches.ContainsKey( label ) )
            {
                HLog.LogWarning( logPrefix, $"No stopwatch found with label {label}" );
                return;
            }

            stopwatches[label].Stop();
            PrintStopWatchInSeconds( label );
        }

        public static void PrintStopWatchInSeconds( string label )
        {
            float timeValue = 0;

            if ( stopwatches != null && stopwatches.ContainsKey( label ) )
            {
                timeValue = stopwatches[label].ElapsedMilliseconds / MILLISECONDS_IN_SECOND;
            }

            if ( stopwatchesFromStart != null && stopwatchesFromStart.ContainsKey( label ) )
            {
                timeValue = stopwatchesFromStart[label] / MILLISECONDS_IN_SECOND;
            }

            HLog.Log( logPrefix, $"Time measurement for {label} is {timeValue}" );
        }

        public static long GetStopwatchResultInMilliseconds( string label )
        {
            if ( stopwatches != null && stopwatches.ContainsKey( label ) )
            {
                return stopwatches[label].ElapsedMilliseconds;
            }

            if ( stopwatchesFromStart != null && stopwatchesFromStart.ContainsKey( label ) )
            {
                return stopwatchesFromStart[label];
            }

            HLog.LogError( logPrefix, $"Stopwatch with {label} not found" );
            return -1;
        }

        public static float GetStopwatchResultInSeconds( string label )
        {
            return GetStopwatchResultInMilliseconds( label ) / MILLISECONDS_IN_SECOND;
        }

        public static void PrintResultsInSeconds()
        {
            if ( stopwatches == null && stopwatchesFromStart == null )
            {
                HLog.Log( logPrefix, $"Time measurement\nStopwatch Empty" );
                return;
            }

            var results = new StringBuilder( $"Time measurement in seconds\n" );

            if ( stopwatches != null )
            {
                foreach ( var stopwatch in stopwatches )
                {
                    results.Append( $"{stopwatch.Key}: {stopwatch.Value.ElapsedMilliseconds / MILLISECONDS_IN_SECOND}" +
                                    $"{( stopwatch.Value.IsRunning ? " running" : string.Empty )}\n" );
                }
            }

            if ( stopwatchesFromStart != null )
            {
                foreach ( var stopwatch in stopwatchesFromStart )
                {
                    results.Append( $"{stopwatch.Key}: {stopwatch.Value / MILLISECONDS_IN_SECOND}\n" );
                }
            }

            HLog.Log( logPrefix, $" {results.ToString()}" );
        }

        public static void PrintResultsInMilliseconds()
        {
            if ( stopwatches == null && stopwatchesFromStart == null )
            {
                HLog.Log( logPrefix, $" Time measurement\nStopwatch Empty" );
                return;
            }

            var results = new StringBuilder( $"Time measurement in milliseconds\n" );

            if ( stopwatches != null )
            {
                foreach ( var stopwatch in stopwatches )
                {
                    results.Append( $"{stopwatch.Key}: {stopwatch.Value.ElapsedMilliseconds}" +
                                    $"{( stopwatch.Value.IsRunning ? " running" : string.Empty )}\n" );
                }
            }

            if ( stopwatchesFromStart != null )
            {
                foreach ( var stopwatch in stopwatchesFromStart )
                {
                    results.Append( $"{stopwatch.Key}: {stopwatch.Value}\n" );
                }
            }

            HLog.Log( logPrefix, $" {results.ToString()}" );
        }

        public static void PrintFormattedResultsInSeconds()
        {
            if ( stopwatches == null && stopwatchesFromStart == null )
            {
                HLog.Log( logPrefix, $"Time measurement\nStopwatch Empty" );
                return;
            }

            var results = GetLabels( true );

            if ( stopwatches != null )
            {
                foreach ( var stopwatch in stopwatches )
                {
                    results.Append( $"{stopwatch.Value.ElapsedMilliseconds / MILLISECONDS_IN_SECOND}\t" );
                }
            }

            if ( stopwatchesFromStart != null )
            {
                foreach ( var stopwatch in stopwatchesFromStart )
                {
                    results.Append( $"{stopwatch.Value / MILLISECONDS_IN_SECOND}\t" );
                }
            }

            HLog.Log( logPrefix, $" {results.ToString()}" );
        }

        public static void PrintFormattedResultsInMilliseconds()
        {
            if ( stopwatches == null && stopwatchesFromStart == null )
            {
                HLog.Log( logPrefix, $" Time measurement\nStopwatch Empty" );
                return;
            }

            var results = GetLabels();

            if ( stopwatches != null )
            {
                foreach ( var stopwatch in stopwatches )
                {
                    results.Append( $"{stopwatch.Value.ElapsedMilliseconds}\t" );
                }
            }

            if ( stopwatchesFromStart != null )
            {
                foreach ( var stopwatch in stopwatchesFromStart )
                {
                    results.Append( $"{stopwatch.Value}\t" );
                }
            }

            HLog.Log( logPrefix, $" {results.ToString()}" );
        }

        static StringBuilder GetLabels( bool isInSeconds = false )
        {
            var labels = new StringBuilder( $"Time measurement formatted" );
            labels.Append( isInSeconds ? "in seconds" : "in milliseconds" );
            labels.Append( "\n" );

            if ( stopwatches != null )
            {
                foreach ( var stopwatch in stopwatches )
                {
                    labels.Append( $"{stopwatch.Key}\t" );
                }
            }

            if ( stopwatchesFromStart != null )
            {
                foreach ( var stopwatch in stopwatchesFromStart )
                {
                    labels.Append( $"{stopwatch.Key}\t" );
                }
            }

            labels.Append( "\n" );
            return labels;
        }
    }
}