using System.Collections.Generic;
using ARDE.RealEstate3D.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.UI
{
    public class UnitsController : MonoBehaviour
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private List<UnitData> units = new List<UnitData>
        {
            new UnitData("1A", "2 ambientes", "48 m²", "Disponible", "USD 78.000"),
            new UnitData("2B", "3 ambientes", "72 m²", "Disponible", "USD 112.000"),
            new UnitData("3C", "Monoambiente", "34 m²", "Reservada", "USD 59.000"),
            new UnitData("4D", "2 ambientes", "51 m²", "Disponible", "USD 82.000")
        };

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
                GameObject card = cardPrefab != null ? Instantiate(cardPrefab, listContainer) : CreateDefaultCard(listContainer);
                Text[] texts = card.GetComponentsInChildren<Text>(true);
                string cardText = $"{unit.Code}\n{unit.Typology} · {unit.Area}\n{unit.Status}\n{unit.Price}";

                if (texts.Length > 0)
                {
                    texts[0].text = cardText;
                }
            }
        }

        private void ClearContainer()
        {
            for (int i = listContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(listContainer.GetChild(i).gameObject);
            }
        }

        private static GameObject CreateDefaultCard(Transform parent)
        {
            GameObject card = new GameObject("UnitCard", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(parent, false);
            card.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.92f);

            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(card.transform, false);
            Text text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = new Color(0.08f, 0.08f, 0.08f);
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleLeft;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(24f, 12f);
            textRect.offsetMax = new Vector2(-24f, -12f);

            return card;
        }
    }
}
