using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    [RequireComponent(typeof(Button))]
    public class WhatsAppButton : MonoBehaviour
    {
        [SerializeField] private string phoneNumber = "5491112345678";
        [TextArea(2, 4)]
        [SerializeField] private string preloadedMessage = "Hola, quiero recibir información sobre Residencias Norte.";

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenWhatsApp);
        }

        public void OpenWhatsApp()
        {
            string encodedMessage = UnityEngine.Networking.UnityWebRequest.EscapeURL(preloadedMessage);
            Application.OpenURL($"https://wa.me/{phoneNumber}?text={encodedMessage}");
        }
    }
}
