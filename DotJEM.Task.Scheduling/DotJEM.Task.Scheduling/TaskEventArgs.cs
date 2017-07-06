using System;
using DotJEM.Task.Scheduling.Tasks;

namespace DotJEM.Task.Scheduling
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