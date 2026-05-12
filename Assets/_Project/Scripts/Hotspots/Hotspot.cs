using ARDE.RealEstate3D.Data;
using ARDE.RealEstate3D.UI;
using UnityEngine;

namespace ARDE.RealEstate3D.Hotspots
{
    [RequireComponent(typeof(Collider))]
    public class Hotspot : MonoBehaviour
    {
        [SerializeField] private HotspotData data;
        [SerializeField] private InfoPanelController infoPanel;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmount = 0.055f;
        [SerializeField] private float selectedScaleMultiplier = 1.18f;
        [SerializeField] private Color normalColor = new Color(0.95f, 0.68f, 0.18f, 1f);
        [SerializeField] private Color selectedColor = new Color(1f, 0.88f, 0.45f, 1f);

        private static Hotspot activeHotspot;

        private Renderer[] visualRenderers;
        private Vector3 initialScale;
        private bool isSelected;

        public HotspotData Data => data;

        private void Awake()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            initialScale = visualRoot.localScale;
            visualRenderers = visualRoot.GetComponentsInChildren<Renderer>(true);
            ApplySelectionState(false);
        }

        private void OnDisable()
        {
            if (activeHotspot == this)
            {
                activeHotspot = null;
            }
        }

        private void Update()
        {
            if (visualRoot == null)
            {
                return;
            }

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            float selectedMultiplier = isSelected ? selectedScaleMultiplier : 1f;
            visualRoot.localScale = initialScale * pulse * selectedMultiplier;
        }

        public void Configure(HotspotData hotspotData, InfoPanelController panel, Transform visual = null)
        {
            data = hotspotData;
            infoPanel = panel;

            if (visual != null)
            {
                visualRoot = visual;
            }
        }

        public void Select()
        {
            if (data == null)
            {
                Debug.LogWarning($"Hotspot sin datos configurados: {name}", this);
                return;
            }

            if (activeHotspot != null && activeHotspot != this)
            {
                activeHotspot.ApplySelectionState(false);
            }

            activeHotspot = this;
            ApplySelectionState(true);

            if (infoPanel == null)
            {
                infoPanel = FindObjectOfType<InfoPanelController>();
            }

            if (infoPanel != null)
            {
                infoPanel.Show(data);
            }
        }

        private void ApplySelectionState(bool selected)
        {
            isSelected = selected;

            if (visualRenderers == null)
            {
                return;
            }

            Color targetColor = selected ? selectedColor : normalColor;
            foreach (Renderer visualRenderer in visualRenderers)
            {
                if (visualRenderer != null && visualRenderer.material != null)
                {
                    visualRenderer.material.color = targetColor;
                }
            }
        }
    }
}
