using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.Core
{
    public static class Command
    {
        public class Base
        {
            Base executeAfter = null;

            public string data = string.Empty;
            public bool lastResult = false;
            public UnityAction<bool, string> OnComplete;
        
            public virtual void Execute() {}

            public void SetNextCommand(Base command)
            {
                executeAfter = command;
            }
            
            protected virtual void Complete( bool result, string serializedData = "" )
            {
                this.Log( $"Completed with result: {result.ToString()} and data: '{serializedData}'." );
                OnComplete?.Invoke( result, serializedData );

                if ( executeAfter != null )
                {
                    executeAfter.data = serializedData;
                    executeAfter.lastResult = result;
                    Command.Execute( executeAfter );
                }
            }
        }
        
        static readonly Queue<Base> commandQueue = new Queue<Base>();

        public static bool QueueIsEmpty => commandQueue.Count == 0;
        
        public static void Execute( Base command )
        {
            command.Log( "Execute as default." );
            command?.Execute();
        }

        public static void FlushQueue()
        {
            if ( commandQueue.Count > 0 )
            {
                BindAndExecute( commandQueue.ToArray() );
                commandQueue.Clear();
            }
        }
        
        public static void BindAndExecute(params Base[] commands)
        {
            var commandsCount = commands.Length;

            if ( commandsCount == 1 )
            {
                Execute( commands[0] );
                return;
            }

            for ( int i = 0; i < commands.Length - 1; i++ )
            {
                commands[i].SetNextCommand( commands[i + 1] );
            }

            Execute( commands[0] );
        }

        public static void Enqueue( Base command )
        {
            commandQueue.Enqueue( command );
            command.Log( $"Command enqueued {commandQueue.Count}." );
        }
    }
}
