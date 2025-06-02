using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasySaveV3._0.EasySave.ThreadManagement
{
    public class ThreadManager
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<Func<Task>> _taskQueue = new();
        private bool _isRunning = false;

        public ThreadManager(int maxParallelJobs)
        {
            _semaphore = new SemaphoreSlim(maxParallelJobs, maxParallelJobs);
        }

        public void Enqueue(Func<Task> task)
        {
            _taskQueue.Enqueue(task);
            if (!_isRunning)
            {
                _isRunning = true;
                Task.Run(ProcessQueue);
            }
        }

        private async Task ProcessQueue()
        {
            while (_taskQueue.TryDequeue(out var task))
            {
                await _semaphore.WaitAsync();

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await task();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });
            }
            _isRunning = false;
        }
    }
}