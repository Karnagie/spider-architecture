using Infrastructure.Disposables;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Dev
{
    public class TestButton : MonoBehaviour
    {
        public Button Button;
        
        private IClearable _clearable;

        [Inject]
        private void Construct(IClearable clearable)
        {
            _clearable = clearable;
        }

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _clearable.Clear().ThatWithTime(DisposeTime.SceneEnd);
        }
    }
}