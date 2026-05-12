using ARDE.RealEstate3D.Core;
using ARDE.RealEstate3D.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    public class InfoPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text detailsText;
        [SerializeField] private Button consultButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private UnityEvent onConsultRequested;
        [SerializeField] private ExperienceManager experienceManager;

        private void Awake()
        {
            if (consultButton != null)
            {
                consultButton.onClick.AddListener(Consult);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (experienceManager == null)
            {
                experienceManager = FindObjectOfType<ExperienceManager>();
            }

            Hide();
        }

        public void Show(HotspotData data)
        {
            if (data == null)
            {
                return;
            }

            if (titleText != null) titleText.text = data.Title;
            if (descriptionText != null) descriptionText.text = data.Description;
            if (detailsText != null) detailsText.text = data.GetFormattedDetails();

            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Consult()
        {
            onConsultRequested?.Invoke();

            if (experienceManager != null)
            {
                experienceManager.OpenConsultation();
            }
        }
    }
}
