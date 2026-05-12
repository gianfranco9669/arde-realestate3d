using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    public class QRPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text messageText;
        [SerializeField] private Button whatsappButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private string phoneNumber = "5491112345678";
        [TextArea(2, 4)]
        [SerializeField] private string preloadedMessage = "Hola, quiero recibir información sobre Residencias Norte.";

        private void Awake()
        {
            if (whatsappButton != null) whatsappButton.onClick.AddListener(OpenWhatsApp);
            if (closeButton != null) closeButton.onClick.AddListener(Hide);

            if (titleText != null) titleText.text = "Contactanos por WhatsApp";
            if (bodyText != null) bodyText.text = "Escaneá el QR o tocá el botón para hablar con un asesor comercial.";
            if (messageText != null) messageText.text = $"Mensaje: {preloadedMessage}";

            Hide();
        }

        public void Show()
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            else gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            else gameObject.SetActive(false);
        }

        public void OpenWhatsApp()
        {
            string encodedMessage = UnityEngine.Networking.UnityWebRequest.EscapeURL(preloadedMessage);
            Application.OpenURL($"https://wa.me/{phoneNumber}?text={encodedMessage}");
        }
    }
}
