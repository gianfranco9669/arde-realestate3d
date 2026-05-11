using ARDE.RealEstate3D.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        [SerializeField] private ExperienceManager experienceManager;
        [SerializeField] private ExperienceSection targetSection;

        private void Awake()
        {
            if (experienceManager == null)
            {
                experienceManager = FindObjectOfType<ExperienceManager>();
            }

            GetComponent<Button>().onClick.AddListener(Navigate);
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
    }
}
