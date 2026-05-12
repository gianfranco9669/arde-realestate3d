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
            card.GetComponent<Image>().color = new Color(0.98f, 0.96f, 0.9f, 0.98f);
            LayoutElement layoutElement = card.GetComponent<LayoutElement>();
            layoutElement.minHeight = 132f;
            layoutElement.preferredHeight = 142f;

            HorizontalLayoutGroup layout = card.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 18, 18);
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = false;

            Text code = CreateText(card.transform, unit.Code, 42, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.1f, 0.08f, 0.06f));
            code.gameObject.AddComponent<LayoutElement>().preferredWidth = 120f;

            Text details = CreateText(card.transform, $"{unit.Typology}\n{unit.Area}", 28, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.13f, 0.12f, 0.1f));
            details.gameObject.AddComponent<LayoutElement>().preferredWidth = 430f;

            GameObject badge = new GameObject("Estado", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            badge.transform.SetParent(card.transform, false);
            badge.GetComponent<Image>().color = GetStatusColor(unit.Status);
            badge.GetComponent<LayoutElement>().preferredWidth = 190f;
            CreateText(badge.transform, unit.Status, 26, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);

            Text price = CreateText(card.transform, unit.Price, 31, FontStyle.Bold, TextAnchor.MiddleRight, new Color(0.1f, 0.08f, 0.06f));
            price.gameObject.AddComponent<LayoutElement>().preferredWidth = 240f;

            Button consult = CreateButton("Consultar", card.transform, new Color(0.72f, 0.58f, 0.34f));
            consult.gameObject.AddComponent<LayoutElement>().preferredWidth = 210f;
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
