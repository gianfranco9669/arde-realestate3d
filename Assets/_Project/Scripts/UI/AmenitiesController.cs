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
                GameObject card = cardPrefab != null ? Instantiate(cardPrefab, gridContainer) : CreateDefaultCard(gridContainer);
                Text[] texts = card.GetComponentsInChildren<Text>(true);

                if (texts.Length > 0)
                {
                    texts[0].text = string.IsNullOrWhiteSpace(amenity.Description)
                        ? amenity.Title
                        : $"{amenity.Title}\n{amenity.Description}";
                }
            }
        }

        private static GameObject CreateDefaultCard(Transform parent)
        {
            GameObject card = new GameObject("AmenityCard", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(parent, false);
            card.GetComponent<Image>().color = new Color(0.93f, 0.88f, 0.78f, 0.95f);

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(card.transform, false);
            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = new Color(0.1f, 0.08f, 0.05f);
            text.fontSize = 30;
            text.alignment = TextAnchor.MiddleCenter;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(18f, 12f);
            textRect.offsetMax = new Vector2(-18f, -12f);

            return card;
        }
    }
}
