using System;

namespace DotJEM.Tasks.Scheduling.Tasks
{
    public class SingleFireScheduledTask : ScheduledTask
    {
        private readonly TimeSpan delay;

        public override IScheduledTask Start()
        {
            return RegisterWait(delay);
        }

        public SingleFireScheduledTask(string name, Action<bool> callback, TimeSpan? delay)
            : base(name, callback)
        {
            this.delay = delay ?? TimeSpan.Zero;
        }
    }
}