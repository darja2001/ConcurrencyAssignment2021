using System;
using System.Threading;
using System.Threading.Tasks;
using Decoder;
using Task = Decoder.Task;

namespace ConcDecoder
{
    /// <summary>
    /// A concurrent version of the class Buffer
    /// Note: For the final solution this class MUST be implemented.
    /// </summary>
    public class ConcurrentTaskBuffer : TaskBuffer
    {
        //todo: add required fields such that satisfies a thread safe shared buffer.

        public ConcurrentTaskBuffer() : base()
        {

            //todo: implement this method such that satisfies a thread safe shared buffer.
        }

        /// <summary>
        /// Adds the given task to the queue. The implementation must support concurrent accesses.
        /// </summary>
        /// <param name="task">A task to wait in the queue for the execution</param>
        public override void AddTask(TaskDecryption task)
        {
            lock(this) {
            Thread thread = new Thread(() => this.taskBuffer.Enqueue(task));
            thread.Start();
             
            
            this.numOfTasks++;
            this.maxBuffSize = this.taskBuffer.Count > this.maxBuffSize ? this.taskBuffer.Count  : this.maxBuffSize;

            this.LogVisualisation();
            this.PrintBufferSize();
                }
            //todo: implement this method such that satisfies a thread safe shared buffer.
        }

        /// <summary>
        /// Picks the next task to be executed. The implementation must support concurrent accesses.
        /// </summary>
        /// <returns>Next task from the list to be executed. Null if there is no task.</returns>
        public override TaskDecryption GetNextTask()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            TaskDecryption t = null;
             if (this.taskBuffer.Count > 0)
            {
                t = this.taskBuffer.Dequeue();
                // check if the task is the last ending task: put the task back.
                // It is an indication to terminate processors
                if (t.id < 0)
                    this.taskBuffer.Enqueue(t);
            }
            return t;
        }

        /// <summary>
        /// Prints the number of elements available in the buffer.
        /// </summary>
        public override void PrintBufferSize()
        {
            lock(this) { 
            Console.Write("Buffer#{0} ; ", this.taskBuffer.Count);
            }
            //todo: implement this method such that satisfies a thread safe shared buffer.
        }
    }

    class ConcLaunch : Launch
    {
        public ConcLaunch() : base(){  }

        /// <summary>
        /// This method implements the concurrent version of the decryption of provided challenges.
        /// </summary>
        /// <param name="numOfProviders">Number of providers</param>
        /// <param name="numOfWorkers">Number of workers</param>
        /// <returns>Information logged during the execution.</returns>
        
        public string ConcurrentTaskExecution(int numOfProviders, int numOfWorkers)
        {
            ConcurrentTaskBuffer tasks = new ConcurrentTaskBuffer();
            Provider provider = new Provider(tasks, challenges);
            Worker worker = new Worker(tasks);

            Thread thread = new Thread(() => provider.SendTasks());
            Thread thread1 = new Thread(() => worker.ExecuteTasks());
            thread.Start();
            //provider.SendTasks();
            thread1.Start();

            thread.Join();
            thread1.Join();       
            //todo: implement this method such that satisfies a thread safe shared buffer.

            return tasks.GetLogs();
        }
        // public string ConcurrentTaskExecution(int numOfProviders, int numOfWorkers)
        // {
        //     bool isDone = false;
        //     string log = "";
        //     ConcurrentTaskBuffer tasks = new ConcurrentTaskBuffer();
            
        //     Provider provider = new Provider(tasks, this.challenges);
        //     Worker worker = new Worker(tasks);

        //     provider.SendTasks();
        //     worker.ExecuteTasks();

        //     System.Threading.Tasks.Task[] tasks1 = new System.Threading.Tasks.Task[numOfWorkers];
        //     for (int i = 0; i < numOfWorkers - 1; i++){
                
        //         tasks1[i] = new TaskFactory().StartNew(() =>  new Worker(tasks).ExecuteTasks());
        //     }

        //     new TaskFactory().ContinueWhenAll(tasks1, completedTasks =>
        //     {
        //         Console.WriteLine("{0} contains: ", "");
        //     });

        //     // todo: implement this method such that satisfies a thread safe shared buffer.


        //     return log;
        // }
    }
}
