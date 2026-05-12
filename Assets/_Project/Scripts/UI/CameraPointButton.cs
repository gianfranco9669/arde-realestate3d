using ARDE.RealEstate3D.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class CameraPointButton : MonoBehaviour
    {
        [SerializeField] private CameraPointController cameraPointController;
        [SerializeField] private int pointIndex;
        [SerializeField] private Color normalColor = new Color(0.95f, 0.91f, 0.82f, 0.95f);
        [SerializeField] private Color activeColor = new Color(0.72f, 0.58f, 0.34f, 1f);
        [SerializeField] private Color normalTextColor = new Color(0.12f, 0.1f, 0.08f, 1f);
        [SerializeField] private Color activeTextColor = Color.white;

        private Image background;
        private Text label;

        private void Awake()
        {
            background = GetComponent<Image>();
            label = GetComponentInChildren<Text>(true);

            if (cameraPointController == null)
            {
                cameraPointController = FindObjectOfType<CameraPointController>();
            }

            GetComponent<Button>().onClick.AddListener(GoToPoint);
        }

        private void OnEnable()
        {
            if (cameraPointController != null)
            {
                cameraPointController.CameraPointChanged += UpdateVisualState;
                UpdateVisualState(cameraPointController.CurrentIndex);
            }
        }

        private void OnDisable()
        {
            if (cameraPointController != null)
            {
                cameraPointController.CameraPointChanged -= UpdateVisualState;
            }
        }

        public void Configure(CameraPointController controller, int index)
        {
            cameraPointController = controller;
            pointIndex = index;
        }

        public void GoToPoint()
        {
            if (cameraPointController != null)
            {
                cameraPointController.MoveToPoint(pointIndex);
            }
        }

        private void UpdateVisualState(int activeIndex)
        {
            bool active = activeIndex == pointIndex;

            if (background != null)
            {
                background.color = active ? activeColor : normalColor;
            }

            if (label != null)
            {
                label.color = active ? activeTextColor : normalTextColor;
            }
        }
    }
}
