using Data;
using MVP.Binders;
using MVP.Model;
using MVP.Presenter;

namespace MVP.Factory
{
    public class SpiderFactory
    {
        private SpiderViewFactory _viewFactory;
        private PositionService _positionService;
        private BinderFactory _binderFactory;
        private ObjectMoverFactory _objectMoverFactory;

        public SpiderFactory(SpiderViewFactory viewFactory, ObjectMoverFactory objectMoverFactory, BinderFactory binderFactory)
        {
            _objectMoverFactory = objectMoverFactory;
            _binderFactory = binderFactory;
            _viewFactory = viewFactory;
        }

        public Spider Create()
        {
            var view = _viewFactory.DefalutSpider();
            var model = new Spider(Ids.GetNewId());
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