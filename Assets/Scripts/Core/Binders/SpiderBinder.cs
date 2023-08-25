using Core.Models;
using Infrastructure.Disposables;

namespace Core.Binders
{
    public class SpiderBinder : Binder
    {
        private SpiderView _view;
        private Spider _model;

        public SpiderBinder(SpiderView view, Spider model)
        {
            _model = model;
            _view = view;
        }

        public override void Bind()
        {
            Bind(_model.Stats.Health, _view.ChangeHealth);
        }

        public override DisposeTime DisposeTime => DisposeTime.SceneEnd;
    }
}