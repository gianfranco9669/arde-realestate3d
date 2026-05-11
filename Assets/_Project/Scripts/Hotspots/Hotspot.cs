using ARDE.RealEstate3D.Data;
using ARDE.RealEstate3D.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARDE.RealEstate3D.Hotspots
{
    [RequireComponent(typeof(Collider))]
    public class Hotspot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private HotspotData data;
        [SerializeField] private InfoPanelController infoPanel;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmount = 0.08f;

        private Vector3 initialScale;

        public HotspotData Data => data;

        private void Awake()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            initialScale = visualRoot.localScale;
        }

        private void Update()
        {
            if (visualRoot == null)
            {
                return;
            }

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            visualRoot.localScale = initialScale * pulse;
        }

        private void OnMouseDown()
        {
            Select();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void Configure(HotspotData hotspotData, InfoPanelController panel)
        {
            data = hotspotData;
            infoPanel = panel;
        }

        public void Select()
        {
            if (data == null)
            {
                Debug.LogWarning($"Hotspot sin datos configurados: {name}", this);
                return;
            }

            if (infoPanel == null)
            {
                infoPanel = FindObjectOfType<InfoPanelController>();
            }

            if (infoPanel != null)
            {
                infoPanel.Show(data);
            }
        }
    }
}
