using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Behaviours
{
    public class SpiderBehaviour : MonoBehaviour
    {
        public TMP_Text HealthText;
        public Transform LegLeft;
        public Transform LegRight;
        
        public Transform Transform;
        public Collider2D Collider;
        public Rigidbody2D Rigidbody;
    }
}