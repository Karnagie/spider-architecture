using UnityEditor;
using UnityEngine;

namespace Infrastructure.Kinematics
{
    public class IKFabric : MonoBehaviour
    {
        [SerializeField] private int _length = 2;
        
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _pole;
        
        [SerializeField] private int _iterations = 10;
        [SerializeField] private float _delta = 0.001f;
        
        [Range(0, 1)]
        [SerializeField] private int _snapBackStrength = 1;

        private float[] _bonesLength;
        private float _completeLength;
        private Transform[] _bones;
        private Vector3[] _positions;

        private Vector3[] _startDirectionSucc;
        private Quaternion[] _startRotateBone;
        private Quaternion _startRotationTarget;
        private Quaternion _startRotationRoot;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _bones = new Transform[_length + 1];
            _positions = new Vector3[_length + 1];
            _bonesLength = new float[_length];
            _startDirectionSucc = new Vector3[_length + 1];
            _startRotateBone = new Quaternion[_length + 1];

            _startRotationTarget = _target.rotation;
            _completeLength = 0;

            var current = transform;
            for (int i = _bones.Length-1; i >= 0; i--)
            {
                _bones[i] = current;
                _startRotateBone[i] = current.rotation;

                if (i == _bones.Length - 1)
                {
                    _startDirectionSucc[i] = _target.position - current.position;
                }
                else
                {
                    _startDirectionSucc[i] = _bones[i + 1].position - current.position;
                    _bonesLength[i] = _startDirectionSucc[i].magnitude;
                    _completeLength += _bonesLength[i];
                }
                
                current = current.parent;
            }
        }

        private void LateUpdate()
        {
            ReleaseIK();
        }

        //todo refactor
        private void ReleaseIK()
        {
            for (int i = 0; i < _bones.Length; i++)
                _positions[i] = _bones[i].position;

            var rootRot = (_bones[0].parent) != null ? _bones[0].parent.rotation : Quaternion.identity;
            var rootRotDiff = rootRot * Quaternion.Inverse(_startRotationRoot);

            if ((_target.position - _bones[0].position).sqrMagnitude >= _completeLength * _completeLength)
            {
                var direction = (_target.position - _positions[0]).normalized;

                for (int i = 1; i < _positions.Length; i++)
                    _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
            }
            else
            {
                for (int i = 0; i < _positions.Length - 1; i++)
                {
                    _positions[i + 1] = Vector3.Lerp(_positions[i + 1],
                        _positions[i] + rootRotDiff * _startDirectionSucc[i], _snapBackStrength);
                }
                
                for (int iteration = 0; iteration < _iterations; iteration++)
                {
                    for (int i =  _positions.Length-1; i > 0; i--)
                    {
                        if (i == _positions.Length - 1)
                        {
                            _positions[i] = _target.position;
                        }
                        else
                        {
                            _positions[i] = _positions[i + 1] +
                                            (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i];
                        }
                    }

                    for (int i = 1; i < _positions.Length; i++)
                        _positions[i] = _positions[i - 1] +
                                        (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i-1];

                    if ((_positions[^1] - _target.position).sqrMagnitude < _delta * _delta)
                    {
                         break;
                    }
                }
            }

            if (_pole != null)
            {
                for (int i = 1; i < _positions.Length-1; i++)
                {
                    var plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                    var projectilePole = plane.ClosestPointOnPlane(_pole.position);
                    var projectileBone = plane.ClosestPointOnPlane(_positions[i]);
                    var angle = Vector3.SignedAngle(projectileBone - _positions[i - 1],
                        projectilePole - _positions[i - 1], plane.normal);

                    _positions[i] =
                       Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) +
                        _positions[i - 1];

                }
            }

            for (int i = 0; i < _positions.Length; i++)
            {
                if (i == _positions.Length - 1)
                {
                    _bones[i].rotation = _target.rotation * Quaternion.Inverse(_startRotationTarget) *
                                         _startRotateBone[i];
                }
                else
                {
                    _bones[i].rotation =
                        Quaternion.FromToRotation(_startDirectionSucc[i], _positions[i + 1] - _positions[i]) *
                        _startRotateBone[i];
                }
                _bones[i].position = _positions[i];
            }
        }

        private void OnDrawGizmos()
        {
            var current = transform;

            for (int i = 0; i < _length && current.parent != null; i++)
            {
                var parentPosition = current.parent.position;
                var currentPosition = current.position;
                var scale = Vector3.Distance(currentPosition, parentPosition) * 0.1f;
                
                Handles.matrix = Matrix4x4.TRS(
                    currentPosition,
                    Quaternion.FromToRotation(Vector3.up, parentPosition - currentPosition),
                    new Vector3(scale, Vector3.Distance(parentPosition, currentPosition), scale)
                    );
                Handles.color = Color.green;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                
                current = current.parent;
            }
        }
    }
}