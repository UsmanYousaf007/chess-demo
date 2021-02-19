using System;
using HUF.Storage.Runtime.Implementation.Structs;

namespace HUF.Storage.Runtime.API.Services
{
    public interface IObjectStorage<T> where T : class
    {
        void Get(string filePath, Action<ObjectResultContainer<T>> resultHandler);
        void Download(string filePath, Action<ObjectResultContainer<T>> resultHandler);
    }
}