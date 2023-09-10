using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Behaviours
{
    public class SpiderBehaviour : MonoBehaviour
    {
        public GameObject Body;
        public TMP_Text HealthText;
        public TMP_Text TimerText;
        public Button Button;
        public Canvas Canvas;
        
        public Transform Transform;
        public Collider2D Collider;
    }
}