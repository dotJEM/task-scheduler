using System;

namespace DotJEM.Tasks.Scheduling.Tasks
{
    public interface IScheduledTask : IDisposable
    {
        event EventHandler<TaskEventArgs> TaskCompleted;
        event EventHandler<TaskExceptionEventArgs> TaskException;

        Guid Id { get; }
        string Name { get; }

        IScheduledTask Start();
        IScheduledTask Signal();
        IScheduledTask Signal(TimeSpan delay);

    }
}