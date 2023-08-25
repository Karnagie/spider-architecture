using System.Collections.Generic;
using Data;
using UniRx;

namespace Infrastructure.Services.Positions
{
    public class PositionService
    {
        private Dictionary<int, IReactiveProperty<Vector3Data>> _objects = new();
        
        public void Add(int id) => 
            _objects.Add(id, new ReactiveProperty<Vector3Data>(new Vector3Data(0, 0, 0)));
        
        public void Add(int id, Vector3Data startPosition) => 
            _objects.Add(id, new ReactiveProperty<Vector3Data>(startPosition));
        
        public IReadOnlyReactiveProperty<Vector3Data> GetPosition(int id) =>
            _objects[id];

        public void AddOffset(int id, Vector3Data offset)
        {
            _objects[id].Value += offset;
        }

        public void SetPosition(int id, Vector3Data newPosition)
        {
            _objects[id].Value = newPosition;
        }
    }
}