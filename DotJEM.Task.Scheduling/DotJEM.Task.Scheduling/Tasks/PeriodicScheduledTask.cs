using System;

namespace DotJEM.Task.Scheduling.Tasks
{
    public class PeriodicScheduledTask : ScheduledTask
    {
        private readonly TimeSpan delay;

        public PeriodicScheduledTask(string name, Action<bool> callback, TimeSpan delay)
            : base(name, callback)
        {
            this.delay = delay;
        }

        public override IScheduledTask Start()
        {
            return RegisterWait(delay);
        }

        protected override bool ExecuteCallback(bool timedout)
        {
            bool success = base.ExecuteCallback(timedout);
            //TODO: Count exceptions, increase callback time if reoccurences.                
            RegisterWait(delay);
            return success;
        }
    }
}