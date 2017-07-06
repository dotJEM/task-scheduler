using System;
using System.Collections.Concurrent;
using DotJEM.Tasks.Scheduling.Tasks;

namespace DotJEM.Tasks.Scheduling
{
    /* inspiration from hangfire API: https://www.hangfire.io/
     * 
     * Fire-and-forget jobs
     *   Fire-and-forget jobs are executed only once and almost immediately after creation.
     *   
     *   var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));
     *   
     *   
     * Delayed jobs
     *   Delayed jobs are executed only once too, but not immediately, after a certain time interval.
     *   
     *   var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromDays(7));
     * 
     * Recurring jobs
     *   Recurring jobs fire many times on the specified CRON schedule.
     * 
     *   RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring!"), Cron.Daily);
     * 
     * Continuations
     *   Continuations are executed when its parent job has been finished.
     * 
     *   BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("Continuation!"));
     * 
     * Batches
     *   Batch is a group of background jobs that is created atomically and considered as a single entity.
     *   
     *   var batchId = BatchJob.StartNew(x => { 
     *     x.Enqueue(() => Console.WriteLine("Job 1"));
     *     x.Enqueue(() => Console.WriteLine("Job 2"));
     *   });
     *   
     * Batch Continuations
     *   Batch continuation is fired when all background jobs in a parent batch finished.
     * 
     *   BatchJob.ContinueWith(batchId, x => { x.Enqueue(() => Console.WriteLine("Last Job")); });
     ***********/

    public interface IScheduler
    {
        IScheduledTask Schedule(IScheduledTask task);
        IScheduledTask ScheduleTask(string name, Action<bool> callback, TimeSpan interval);
        IScheduledTask ScheduleCallback(string name, Action<bool> callback, TimeSpan? timeout = null);
        IScheduledTask ScheduleCron(string name, Action<bool> callback, string trigger);

        void Stop();
    }

    public class StandardScheduler : IScheduler
    {
        private readonly ConcurrentDictionary<Guid, IScheduledTask> tasks = new ConcurrentDictionary<Guid, IScheduledTask>();


        public IScheduledTask Schedule(IScheduledTask task)
        {
            task.TaskException += HandleTaskException;
            task.TaskCompleted += HandleTaskCompleted;
            tasks.TryAdd(task.Id, task);
            return task.Start();
        }

        public IScheduledTask ScheduleTask(string name, Action<bool> callback, TimeSpan interval)
        {
            return Schedule(new PeriodicScheduledTask(name, callback, interval));
        }

        public IScheduledTask ScheduleCallback(string name, Action<bool> callback, TimeSpan? timeout)
        {
            return Schedule(new SingleFireScheduledTask(name, callback, timeout));
        }

        public IScheduledTask ScheduleCron(string name, Action<bool> callback, string trigger)
        {
            return Schedule(new CronScheduledTask(name, callback, trigger));
        }

        private void HandleTaskCompleted(object sender, TaskEventArgs args)
        {
            IScheduledTask task;
            tasks.TryRemove(args.Task.Id, out task);
        }

        public void Stop()
        {
            foreach (IScheduledTask task in tasks.Values)
                task.Dispose();
        }

        private void HandleTaskException(object sender, TaskExceptionEventArgs args)
        {
            try
            {
                //logger.LogException(args.Exception, new
                //{
                //    TaskName = args.Task.Name,
                //    TaskId = args.Task.Id
                //});
            }
            catch (Exception)
            {
                //ignore
                //TODO: (jmd 2015-09-30) Ohh shit, log to the event log or something!. 
            }
        }
    }
}