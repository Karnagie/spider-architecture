using System;
using Core.Models.Commands;
using Core.Models.Components;

namespace Core.Models
{
    public class Spider : ICommandHandler<ISpiderCommand>
    {
        public SpiderStats Stats;
        public SpiderComponents Components;

        public int Id { get; }

        public Spider(SpiderStats stats, SpiderComponents components)
        {
            Stats = stats;
            Components = components;
        }

        public void Perform(ISpiderCommand command)
        {
            command.Perform(this);
        }
    }
}