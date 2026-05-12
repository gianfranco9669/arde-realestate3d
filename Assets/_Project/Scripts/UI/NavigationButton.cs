using ARDE.RealEstate3D.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class NavigationButton : MonoBehaviour
    {
        [SerializeField] private ExperienceManager experienceManager;
        [SerializeField] private ExperienceSection targetSection;
        [SerializeField] private Color normalColor = new Color(0.88f, 0.83f, 0.72f, 0.95f);
        [SerializeField] private Color activeColor = new Color(0.72f, 0.58f, 0.34f, 1f);
        [SerializeField] private Color normalTextColor = new Color(0.10f, 0.09f, 0.075f, 1f);
        [SerializeField] private Color activeTextColor = Color.white;

        private Image background;
        private Text label;

        private void Awake()
        {
            background = GetComponent<Image>();
            label = GetComponentInChildren<Text>(true);

            if (experienceManager == null)
            {
                experienceManager = FindObjectOfType<ExperienceManager>();
            }

            GetComponent<Button>().onClick.AddListener(Navigate);
        }

        private void OnEnable()
        {
            if (experienceManager != null)
            {
                experienceManager.SectionChanged += UpdateVisualState;
                UpdateVisualState(experienceManager.CurrentSection);
            }
        }

        private void OnDisable()
        {
            if (experienceManager != null)
            {
                experienceManager.SectionChanged -= UpdateVisualState;
            }
        }

        public void Configure(ExperienceManager manager, ExperienceSection section)
        {
            experienceManager = manager;
            targetSection = section;
        }

        public void Navigate()
        {
            if (experienceManager != null)
            {
                experienceManager.ShowSection(targetSection);
            }
        }

        private void UpdateVisualState(ExperienceSection activeSection)
        {
            bool active = activeSection == targetSection;

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
