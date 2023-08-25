using CompositeDirectorWithGeneratingComposites.CompositeDirector;
using Core.Models;
using Infrastructure.Disposables;
using Zenject;

namespace Core.Binders
{
    public class BinderFactory
    {
        private readonly IPool<IClearable> _clearables;
        private readonly DiContainer _container;

        public BinderFactory(IPool<IClearable> clearables, DiContainer container)
        {
            _clearables = clearables;
            _container = container;
        }

        public T Create<T>() where T : Binder
        {
            var binder = _container.Instantiate<T>();
            _clearables.Add(binder);
            return binder;
        }

        public SpiderBinder CreateSpiderBinder(SpiderView view, Spider model)
        {
            var binder = new SpiderBinder(view, model);
            _clearables.Add(binder);
            return binder;
        }
    }
}