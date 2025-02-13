using System;
using System.Collections.Generic;
using System.Threading;

namespace MotorControl
{
    public class ThreadManager
    {
        private readonly object lockObject = new object();
        private readonly Queue<Action<CancellationToken>> workerTaskQueue = new Queue<Action<CancellationToken>>();
        private readonly Queue<Action> uiTaskQueue = new Queue<Action>();
        private CancellationTokenSource cancellationTokenSource; // 긴급 정지용 토큰
        private Thread workerThread;
        private readonly Thread uiThread;
        private AutoResetEvent workerTaskSignal = new AutoResetEvent(false);
        private AutoResetEvent uiTaskSignal = new AutoResetEvent(false);
        private bool isRunning = true;

        public ThreadManager()
        {
            workerThread = new Thread(ProcessWorkerTasks) { IsBackground = true };
            workerThread.Start();

            uiThread = new Thread(ProcessUITasks) { IsBackground = true };
            uiThread.Start();
        }

        private void ProcessWorkerTasks()
        {
            while (isRunning)
            {
                workerTaskSignal.WaitOne();
                Action<CancellationToken> taskToExecute = null;

                lock (lockObject)
                {
                    if (workerTaskQueue.Count > 0)
                        taskToExecute = workerTaskQueue.Dequeue();
                }

                if (taskToExecute != null)
                {
                    try
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        taskToExecute(cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"작업 스레드 오류: {ex.Message}");
                    }
                }
            }
        }

        private void ProcessUITasks()
        {
            while (isRunning)
            {
                uiTaskSignal.WaitOne();
                Action taskToExecute = null;

                lock (lockObject)
                {
                    if (uiTaskQueue.Count > 0)
                        taskToExecute = uiTaskQueue.Dequeue();
                }

                taskToExecute?.Invoke();
            }
        }

        public void AddWorkerTask(Action<CancellationToken> task)
        {
            lock (lockObject)
            {
                workerTaskQueue.Enqueue(task);
                workerTaskSignal.Set();
            }
        }

        public void EmergencyStop()
        {
            lock (lockObject)
            {
                cancellationTokenSource?.Cancel();  // 긴급 정지 요청
            }
        }

        public bool IsWorkerRunning()
        {
            lock (lockObject)
            {
                return workerTaskQueue.Count > 0;
            }
        }

        public void Stop()
        {
            isRunning = false;
            workerTaskSignal.Set();
            uiTaskSignal.Set();
            workerThread.Join();
            uiThread.Join();
        }
    }
}
