using UnityEngine;

namespace ARDE.RealEstate3D.Core
{
    public class IdleResetManager : MonoBehaviour
    {
        [SerializeField] private ExperienceManager experienceManager;
        [SerializeField] private float idleSeconds = 45f;

        private float lastInteractionTime;
        private Vector3 lastMousePosition;

        private void Awake()
        {
            if (experienceManager == null)
            {
                experienceManager = FindObjectOfType<ExperienceManager>();
            }

            RegisterInteraction();
            lastMousePosition = Input.mousePosition;
        }

        private void Update()
        {
            if (HasUserInteraction())
            {
                RegisterInteraction();
            }

            if (Time.time - lastInteractionTime >= idleSeconds)
            {
                ResetToHome();
            }
        }

        public void RegisterInteraction()
        {
            lastInteractionTime = Time.time;
        }

        private bool HasUserInteraction()
        {
            bool pointerMoved = (Input.mousePosition - lastMousePosition).sqrMagnitude > 4f;
            lastMousePosition = Input.mousePosition;

            return pointerMoved || Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0;
        }

        private void ResetToHome()
        {
            RegisterInteraction();

            if (experienceManager != null)
            {
                experienceManager.ShowHome();
            }
        }
    }
}
