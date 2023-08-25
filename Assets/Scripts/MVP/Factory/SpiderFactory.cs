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

        public SpiderFactory(SpiderViewFactory viewFactory, PositionService positionService, BinderFactory binderFactory)
        {
            _binderFactory = binderFactory;
            _positionService = positionService;
            _viewFactory = viewFactory;
        }

        public Spider Create()
        {
            var view = _viewFactory.DefalutSpider();
            var model = new Spider(Ids.GetNewId());
            var binder = _binderFactory.CreateSpiderBinder(view, model); 
            binder.Bind();
            
            AddToWorld(model, binder, view);

            return model;
        }

        private void AddToWorld(Spider model, SpiderBinder binder, SpiderView view)
        {
            _positionService.Add(model.Id);
            binder.Bind(_positionService.GetPosition(model.Id), view.ChangePosition);
        }
    }
}