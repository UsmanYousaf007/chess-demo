using System;
using System.Collections.Generic;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using System.Collections;

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

        private Queue<AiMoveRequest> serviceRequestsQueue;
        private AiMoveRequest lastDequeuedMethod;
        private bool taskIsReadyToExecute = true;

        private IPromise<FileRank, FileRank, string> AddToQueue(Action<AiMoveInputVO> function, AiMoveInputVO inputVO)
        {
            if (serviceRequestsQueue == null)
            {
                serviceRequestsQueue = new Queue<AiMoveRequest>();
            }

            var promise = new Promise<FileRank, FileRank, string>();
            var methodToAdd = new AiMoveRequest(function, inputVO, promise);
            serviceRequestsQueue.Enqueue(methodToAdd);
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