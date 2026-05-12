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
        [SerializeField] private Color normalColor = new Color(0.18f, 0.17f, 0.15f, 0.95f);
        [SerializeField] private Color activeColor = new Color(0.72f, 0.58f, 0.34f, 1f);

        private Image background;

        private void Awake()
        {
            background = GetComponent<Image>();

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
            if (background != null)
            {
                background.color = activeSection == targetSection ? activeColor : normalColor;
            }
        }
    }
}
