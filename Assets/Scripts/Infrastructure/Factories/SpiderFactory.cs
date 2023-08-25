using Core.Binders;
using Core.Models;
using Infrastructure.Services.Ids;

namespace Infrastructure.Factories
{
    public class SpiderFactory
    {
        private readonly SpiderViewFactory _viewFactory;
        private readonly BinderFactory _binderFactory;
        private readonly ObjectMoverFactory _objectMoverFactory;

        public SpiderFactory(SpiderViewFactory viewFactory, ObjectMoverFactory objectMoverFactory, BinderFactory binderFactory)
        {
            _objectMoverFactory = objectMoverFactory;
            _binderFactory = binderFactory;
            _viewFactory = viewFactory;
        }

        public Spider Create()
        {
            var view = _viewFactory.DefalutSpider();
            var model = new Spider(IdProvider.GetNewId());
            var binder = _binderFactory.CreateSpiderBinder(view, model); 
            binder.Bind();
            
            AddToWorld(binder, view);

            return model;
        }

        private void AddToWorld(SpiderBinder binder, SpiderView view)
        {
            var inputMover = _objectMoverFactory.CreateInputMover();
            binder.Bind(inputMover.Velocity, view.Move);
        }
    }
}