using System;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using System.Collections;
using Priority_Queue;

namespace TurboLabz.Chess
{
    public partial class ChessAiService
    {
        class AiMoveRequest
        {
            public Action<AiMoveInputVO> function;
            public AiMoveInputVO inputVO;
            public Promise<FileRank, FileRank, string> promise;

            public AiMoveRequest(Action<AiMoveInputVO> function, AiMoveInputVO inputVO, Promise<FileRank, FileRank, string> promise)
            {
                this.function = function;
                this.inputVO = inputVO;
                this.promise = promise;
            }
        }

        private SimplePriorityQueue<AiMoveRequest> serviceRequestsQueue; 
        private AiMoveRequest lastDequeuedMethod;
        private bool taskIsReadyToExecute = true;

        public void AiMoveRequestInit()
        {
            serviceRequestsQueue = new SimplePriorityQueue<AiMoveRequest>();
            lastDequeuedMethod = null;
            taskIsReadyToExecute = true;
        }

        private IPromise<FileRank, FileRank, string> AddToQueue(Action<AiMoveInputVO> function, AiMoveInputVO inputVO, float priority = 0)
        {
            if (serviceRequestsQueue == null)
            {
                serviceRequestsQueue = new SimplePriorityQueue<AiMoveRequest>();
            }

            var promise = new Promise<FileRank, FileRank, string>();
            var methodToAdd = new AiMoveRequest(function, inputVO, promise);
            serviceRequestsQueue.Enqueue(methodToAdd, priority);
            ExecuteQueue();

            return promise;
        }

        private void ExecuteQueue()
        {
            if (serviceRequestsQueue.Count <= 0)
            {
                return;
            }

            routineRunner.StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            yield return null;

            if (taskIsReadyToExecute)
            {
                taskIsReadyToExecute = false;
                lastDequeuedMethod = serviceRequestsQueue.Dequeue();
                lastDequeuedMethod.function(lastDequeuedMethod.inputVO);
            }
        }
    }
}