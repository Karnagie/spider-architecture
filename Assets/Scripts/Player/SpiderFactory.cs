// using Infrastructure.Services.Input;
// using Infrastructure.Services.Ticking;
// using UnityEngine;
// using Zenject;
//
// namespace Player
// {
//     public class SpiderFactory
//     {
//         private IInputService _inputService;
//         private TickService _tickService;
//
//         public SpiderFactory(IInputService inputService, TickService tickService)
//         {
//             _tickService = tickService;
//             _inputService = inputService;
//         }
//         
//         public Spider Get(GameObject gameObject)
//         {
//             var movable = new Movable(gameObject); 
//             var spider = new Spider(movable);
//             
//             var mover = new SpiderMover(_inputService, spider);
//             _tickService.AddTickable(mover);
//             return spider;
//         }
//     }
// }