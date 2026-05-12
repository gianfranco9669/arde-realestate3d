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
            card.GetComponent<Image>().color = new Color(0.98f, 0.96f, 0.90f, 1f);

            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0f, 0.5f);
            cardRect.anchorMax = new Vector2(1f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(0f, 158f);

            LayoutElement layoutElement = card.GetComponent<LayoutElement>();
            layoutElement.minHeight = 150f;
            layoutElement.preferredHeight = 158f;
            layoutElement.flexibleWidth = 1f;

            HorizontalLayoutGroup row = card.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(28, 28, 18, 18);
            row.spacing = 22f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = true;

            CreateTextColumn(card.transform, "Unidad", unit.Code, 128f, 20, 42, TextAnchor.MiddleLeft);
            CreateTextColumn(card.transform, "Tipología", unit.Typology, 300f, 20, 30, TextAnchor.MiddleLeft);
            CreateTextColumn(card.transform, "Superficie", unit.Area, 170f, 20, 30, TextAnchor.MiddleLeft);
            CreateStatusBadge(card.transform, unit.Status, 195f);
            CreateTextColumn(card.transform, "Precio", unit.Price, 245f, 20, 30, TextAnchor.MiddleRight);

            Button consult = CreateButton("Consultar", card.transform, new Color(0.72f, 0.58f, 0.34f));
            LayoutElement consultLayout = consult.gameObject.AddComponent<LayoutElement>();
            consultLayout.minWidth = 205f;
            consultLayout.preferredWidth = 215f;
            consultLayout.minHeight = 80f;
            consultLayout.preferredHeight = 86f;
            consult.onClick.AddListener(onConsult);

            return card;
        }

        private static void CreateTextColumn(Transform parent, string label, string value, float width, int labelSize, int valueSize, TextAnchor alignment)
        {
            GameObject column = new GameObject($"{label}Column", typeof(RectTransform), typeof(LayoutElement), typeof(VerticalLayoutGroup));
            column.transform.SetParent(parent, false);

            LayoutElement layoutElement = column.GetComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.flexibleHeight = 1f;

            VerticalLayoutGroup columnLayout = column.GetComponent<VerticalLayoutGroup>();
            columnLayout.spacing = 2f;
            columnLayout.childAlignment = alignment;
            columnLayout.childControlWidth = true;
            columnLayout.childControlHeight = true;
            columnLayout.childForceExpandWidth = true;
            columnLayout.childForceExpandHeight = false;

            Text labelText = CreateText(column.transform, label, labelSize, FontStyle.Normal, alignment, new Color(0.46f, 0.38f, 0.26f));
            labelText.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;

            Text valueText = CreateText(column.transform, value, valueSize, FontStyle.Bold, alignment, new Color(0.09f, 0.075f, 0.055f));
            valueText.gameObject.AddComponent<LayoutElement>().preferredHeight = 60f;
        }

        private static void CreateStatusBadge(Transform parent, string status, float width)
        {
            GameObject badge = new GameObject("EstadoBadge", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            badge.transform.SetParent(parent, false);
            badge.GetComponent<Image>().color = GetStatusColor(status);

            LayoutElement layoutElement = badge.GetComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = 76f;
            layoutElement.preferredHeight = 82f;

            CreateText(badge.transform, string.IsNullOrWhiteSpace(status) ? "Sin estado" : status, 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        }

        private static Color GetStatusColor(string status)
        {
            string normalized = string.IsNullOrWhiteSpace(status) ? string.Empty : status.ToLowerInvariant();
            if (normalized.Contains("reserv")) return new Color(0.82f, 0.58f, 0.18f);
            if (normalized.Contains("vend")) return new Color(0.48f, 0.48f, 0.5f);
            return new Color(0.16f, 0.55f, 0.32f);
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
            text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return text;
        }

        private static Button CreateButton(string label, Transform parent, Color color)
        {
            GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = color;
            CreateText(buttonObject.transform, label, 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            return buttonObject.GetComponent<Button>();
        }
    }
}
