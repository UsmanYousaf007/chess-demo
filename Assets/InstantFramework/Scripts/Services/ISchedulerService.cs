using System;

namespace TurboLabz.InstantFramework
{
    public interface ISchedulerService
    {
        bool running { get; set; }

        void Init();
        void Start();
        void Stop();
        void Clear();
        void Subscribe(Action callback);
        void UnSubscribe(Action callback);
    }
}