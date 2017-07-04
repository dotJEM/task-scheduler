using System;
using DotJEM.Scheduler.Tasks;

namespace DotJEM.Scheduler
{
    public class TaskEventArgs : EventArgs
    {
        public IScheduledTask Task { get; }

        public TaskEventArgs(IScheduledTask task)
        {
            Task = task;
        }
    }

    public class TaskExceptionEventArgs : TaskEventArgs
    {
        public Exception Exception { get; }

        public TaskExceptionEventArgs(Exception exception, IScheduledTask task, bool seenBefore)
            : base(task)
        {
            Exception = exception;
        }
    }
}