using System;
using ARDE.RealEstate3D.UI;
using UnityEngine;

namespace ARDE.RealEstate3D.Core
{
    public enum ExperienceSection
    {
        Home,
        Experience,
        Units,
        Amenities,
        Contact
    }

    public class ExperienceManager : MonoBehaviour
    {
        [Header("Pantallas principales")]
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject experienceScreen;
        [SerializeField] private GameObject unitsScreen;
        [SerializeField] private GameObject amenitiesScreen;
        [SerializeField] private GameObject contactScreen;

        [Header("Paneles")]
        [SerializeField] private InfoPanelController infoPanel;
        [SerializeField] private QRPanelController qrPanel;

        private ExperienceSection currentSection;

        public event Action<ExperienceSection> SectionChanged;
        public ExperienceSection CurrentSection => currentSection;

        private void Start()
        {
            ShowHome();
        }

        public void ShowHome() => ShowSection(ExperienceSection.Home);
        public void ShowExperience() => ShowSection(ExperienceSection.Experience);
        public void ShowUnits() => ShowSection(ExperienceSection.Units);
        public void ShowAmenities() => ShowSection(ExperienceSection.Amenities);
        public void ShowContact() => ShowSection(ExperienceSection.Contact);

        public void ShowSection(ExperienceSection section)
        {
            currentSection = section;

            SetActive(homeScreen, section == ExperienceSection.Home);
            SetActive(experienceScreen, section == ExperienceSection.Experience);
            SetActive(unitsScreen, section == ExperienceSection.Units);
            SetActive(amenitiesScreen, section == ExperienceSection.Amenities);
            SetActive(contactScreen, section == ExperienceSection.Contact);

            if (section != ExperienceSection.Experience && infoPanel != null)
            {
                infoPanel.Hide();
            }

            if (qrPanel != null)
            {
                qrPanel.Hide();
            }

            SectionChanged?.Invoke(currentSection);
        }

        public void OpenConsultation()
        {
            if (qrPanel != null)
            {
                qrPanel.Show();
            }
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
