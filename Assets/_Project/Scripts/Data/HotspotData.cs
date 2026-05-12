using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARDE.RealEstate3D.Data
{
    [Serializable]
    public class HotspotData
    {
        [SerializeField] private string title;
        [TextArea(2, 5)]
        [SerializeField] private string description;
        [SerializeField] private List<string> details = new List<string>();

        public string Title => title;
        public string Description => description;
        public IReadOnlyList<string> Details => details;

        public HotspotData(string title, string description, IEnumerable<string> details)
        {
            this.title = title;
            this.description = description;
            this.details = new List<string>(details);
        }

        public string GetFormattedDetails()
        {
            return details == null || details.Count == 0 ? string.Empty : "• " + string.Join("\n• ", details);
        }
    }
}
