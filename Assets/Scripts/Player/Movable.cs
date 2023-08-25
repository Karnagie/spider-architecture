// using UnityEngine;
//
// namespace Player
// {
//     public class Movable
//     {
//         private GameObject _gameObject;
//
//         public Movable(GameObject gameObject)
//         {
//             _gameObject = gameObject;
//         }
//
//         public Vector3 Velocity => _gameObject.transform.position;
//
//         public void Move(Vector2 offset)
//         {
//             _gameObject.transform.Translate(offset);
//         }
//
//         public void TeleportTo(Vector3 newPosition) 
//         {
//             _gameObject.transform.position = newPosition;
//         }
//     }
// }