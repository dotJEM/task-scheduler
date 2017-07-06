using System;
using NCrontab;

namespace DotJEM.Tasks.Scheduling.Tasks
{
    public class CronScheduledTask : ScheduledTask
    {
        private readonly CrontabSchedule trigger;

        public CronScheduledTask(string name, Action<bool> callback, string trigger)
            : base(name, callback)
        {
            this.trigger = CrontabSchedule.Parse(trigger);
        }

        public override IScheduledTask Start()
        {
            return RegisterWait(Next());
        }

        private TimeSpan Next()
        {
            return trigger.GetNextOccurrence(DateTime.Now).Subtract(DateTime.Now);
        }

        protected override bool ExecuteCallback(bool timedout)
        {
            bool success = base.ExecuteCallback(timedout);
            //TODO: Count exceptions, increase callback time if reoccurences.                
            RegisterWait(Next());
            return success;
        }


    }
}