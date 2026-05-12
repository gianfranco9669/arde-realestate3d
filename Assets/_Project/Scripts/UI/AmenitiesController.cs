using System.Collections.Generic;
using ARDE.RealEstate3D.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    public class AmenitiesController : MonoBehaviour
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private List<AmenityData> amenities = new List<AmenityData>
        {
            new AmenityData("SUM", "Salón de usos múltiples para encuentros."),
            new AmenityData("Piscina", "Espacio exterior de relax."),
            new AmenityData("Gimnasio", "Área equipada para entrenamiento."),
            new AmenityData("Parrillas", "Sector social con parrillas."),
            new AmenityData("Cocheras", "Opciones de estacionamiento."),
            new AmenityData("Seguridad", "Control de acceso y monitoreo.")
        };

        private void OnEnable()
        {
            BuildGrid();
        }

        public void BuildGrid()
        {
            if (gridContainer == null)
            {
                return;
            }

            for (int i = gridContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(gridContainer.GetChild(i).gameObject);
            }

            foreach (AmenityData amenity in amenities)
            {
                GameObject card = cardPrefab != null ? Instantiate(cardPrefab, gridContainer) : CreateDefaultCard(gridContainer, amenity);
                Text[] texts = card.GetComponentsInChildren<Text>(true);

                if (cardPrefab != null && texts.Length > 0)
                {
                    texts[0].text = string.IsNullOrWhiteSpace(amenity.Description)
                        ? amenity.Title
                        : $"{amenity.Title}\n{amenity.Description}";
                }
            }
        }

        private static GameObject CreateDefaultCard(Transform parent, AmenityData amenity)
        {
            GameObject card = new GameObject($"Amenity {amenity.Title}", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(parent, false);
            card.GetComponent<Image>().color = new Color(0.98f, 0.96f, 0.90f, 1f);

            HorizontalLayoutGroup layout = card.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(26, 26, 22, 22);
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = false;

            string initial = string.IsNullOrWhiteSpace(amenity.Title) ? "·" : amenity.Title.Substring(0, 1).ToUpperInvariant();
            GameObject iconBadge = new GameObject("Icono", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            iconBadge.transform.SetParent(card.transform, false);
            iconBadge.GetComponent<Image>().color = new Color(0.82f, 0.70f, 0.50f);
            iconBadge.GetComponent<LayoutElement>().preferredWidth = 86f;
            CreateText(iconBadge.transform, initial, 46, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);

            Text copy = CreateText(card.transform, $"{amenity.Title}\n{amenity.Description}", 27, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.10f, 0.09f, 0.075f));
            copy.gameObject.AddComponent<LayoutElement>().preferredWidth = 330f;

            return card;
        }

        private static Text CreateText(Transform parent, string value, int size, FontStyle style, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return text;
        }
    }
}
