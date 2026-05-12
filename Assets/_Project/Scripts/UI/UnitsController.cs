using System.Collections.Generic;
using ARDE.RealEstate3D.Core;
using ARDE.RealEstate3D.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    public class UnitsController : MonoBehaviour
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private ExperienceManager experienceManager;
        [SerializeField] private List<UnitData> units = new List<UnitData>
        {
            new UnitData("1A", "2 ambientes", "48 m²", "Disponible", "USD 78.000"),
            new UnitData("2B", "3 ambientes", "72 m²", "Disponible", "USD 112.000"),
            new UnitData("3C", "Monoambiente", "34 m²", "Reservada", "USD 59.000"),
            new UnitData("4D", "2 ambientes", "51 m²", "Disponible", "USD 82.000")
        };

        private void Awake()
        {
            if (experienceManager == null)
            {
                experienceManager = FindObjectOfType<ExperienceManager>();
            }
        }

        private void OnEnable()
        {
            BuildList();
        }

        public void SetUnits(List<UnitData> newUnits)
        {
            units = newUnits ?? new List<UnitData>();
            BuildList();
        }

        public void BuildList()
        {
            if (listContainer == null)
            {
                return;
            }

            ClearContainer();

            foreach (UnitData unit in units)
            {
                GameObject card = cardPrefab != null ? Instantiate(cardPrefab, listContainer) : CreateDefaultCard(listContainer, unit, Consult);
                Text[] texts = card.GetComponentsInChildren<Text>(true);

                if (cardPrefab != null && texts.Length > 0)
                {
                    texts[0].text = $"{unit.Code} · {unit.Typology} · {unit.Area} · {unit.Status} · {unit.Price}";
                }
            }
        }

        private void Consult()
        {
            if (experienceManager != null)
            {
                experienceManager.OpenConsultation();
            }
        }

        private void ClearContainer()
        {
            for (int i = listContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(listContainer.GetChild(i).gameObject);
            }
        }

        private static GameObject CreateDefaultCard(Transform parent, UnitData unit, UnityEngine.Events.UnityAction onConsult)
        {
            GameObject card = new GameObject($"Unidad {unit.Code}", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            card.transform.SetParent(parent, false);
            card.GetComponent<Image>().color = new Color(0.98f, 0.96f, 0.9f, 1f);

            LayoutElement layoutElement = card.GetComponent<LayoutElement>();
            layoutElement.minHeight = 150f;
            layoutElement.preferredHeight = 158f;

            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0f, 0.5f);
            cardRect.anchorMax = new Vector2(1f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);

            CreateText(card.transform, "Unidad", 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.46f, 0.38f, 0.26f), new Vector2(0.035f, 0.58f), new Vector2(0.13f, 0.84f));
            CreateText(card.transform, unit.Code, 42, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.09f, 0.075f, 0.055f), new Vector2(0.035f, 0.16f), new Vector2(0.13f, 0.62f));

            CreateText(card.transform, "Tipología", 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.46f, 0.38f, 0.26f), new Vector2(0.16f, 0.58f), new Vector2(0.37f, 0.84f));
            CreateText(card.transform, unit.Typology, 30, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.11f, 0.095f, 0.075f), new Vector2(0.16f, 0.16f), new Vector2(0.37f, 0.62f));

            CreateText(card.transform, "Superficie", 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.46f, 0.38f, 0.26f), new Vector2(0.40f, 0.58f), new Vector2(0.52f, 0.84f));
            CreateText(card.transform, unit.Area, 30, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.11f, 0.095f, 0.075f), new Vector2(0.40f, 0.16f), new Vector2(0.52f, 0.62f));

            GameObject badge = CreatePanel(card.transform, "EstadoBadge", GetStatusColor(unit.Status), new Vector2(0.56f, 0.30f), new Vector2(0.70f, 0.72f));
            CreateText(badge.transform, unit.Status, 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one);

            CreateText(card.transform, "Precio", 20, FontStyle.Normal, TextAnchor.UpperRight, new Color(0.46f, 0.38f, 0.26f), new Vector2(0.72f, 0.58f), new Vector2(0.84f, 0.84f));
            CreateText(card.transform, unit.Price, 30, FontStyle.Bold, TextAnchor.LowerRight, new Color(0.09f, 0.075f, 0.055f), new Vector2(0.70f, 0.16f), new Vector2(0.84f, 0.62f));

            Button consult = CreateButton("Consultar", card.transform, new Color(0.72f, 0.58f, 0.34f));
            RectTransform consultRect = consult.GetComponent<RectTransform>();
            consultRect.anchorMin = new Vector2(0.865f, 0.26f);
            consultRect.anchorMax = new Vector2(0.975f, 0.74f);
            consultRect.offsetMin = Vector2.zero;
            consultRect.offsetMax = Vector2.zero;
            consult.onClick.AddListener(onConsult);

            return card;
        }

        private static Color GetStatusColor(string status)
        {
            string normalized = string.IsNullOrWhiteSpace(status) ? string.Empty : status.ToLowerInvariant();
            if (normalized.Contains("reserv")) return new Color(0.82f, 0.58f, 0.18f);
            if (normalized.Contains("vend")) return new Color(0.48f, 0.48f, 0.5f);
            return new Color(0.16f, 0.55f, 0.32f);
        }

        private static Text CreateText(Transform parent, string value, int size, FontStyle style, TextAnchor alignment, Color color, Vector2 anchorMin, Vector2 anchorMax)
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
            text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;

            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return panel;
        }

        private static Button CreateButton(string label, Transform parent, Color color)
        {
            GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = color;
            CreateText(buttonObject.transform, label, 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one);
            return buttonObject.GetComponent<Button>();
        }
    }
}
