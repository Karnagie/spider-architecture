using System;
using Core.Models.Commands;
using Core.Models.Components;
using Core.Models.Systems;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;

namespace Core.Models
{
    public class Spider : ICommandHandler<ISpiderCommand>, ISingleSystem
    {
        public SpiderStats Stats;
        public SpiderComponents Components;

        public Observable Killed = new(); 

        public Spider(SpiderStats stats, SpiderComponents components)
        {
            Stats = stats;
            Components = components;
        }

        public void Perform(ISpiderCommand command)
        {
            command.Perform(this);
        }

        public void Kill()
        {
            Killed.Invoke();
        }
    }
}