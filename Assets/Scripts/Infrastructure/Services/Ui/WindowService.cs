using Infrastructure.States;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.Ui
{
    public class WindowService
    {
        private DiContainer _container;

        public WindowService(DiContainer container)
        {
            _container = container;
        }
        
        public MainMenuWindow CreateMainMenu()
        {
            var window = _container.InstantiatePrefabResourceForComponent<MainMenuWindow>("UI/MainMenu");
            return window;
        }

        public MainMenuWindow CreateLevelMenu()
        {
            var window = _container.InstantiatePrefabResourceForComponent<MainMenuWindow>("UI/MainMenu");
            return window;
        }
    }
}