using System;
using Core.Models.Commands;
using Core.Models.Components;
using Core.Models.Systems;
using Infrastructure.Helpers;
using Infrastructure.Services.Binding;

namespace Core.Models
{
    public class Spider : ISpider, ICommandHandler<ISpiderCommand>
    {
        public SpiderStats Stats { get; }
        public SpiderComponents Components { get; }

        public Observable Killed { get; } = new();

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

    public interface ISpider : ISingleSystem
    {
        SpiderStats Stats { get; }
        SpiderComponents Components { get; }

        Observable Killed { get; }
        
        void Kill();
    }
}