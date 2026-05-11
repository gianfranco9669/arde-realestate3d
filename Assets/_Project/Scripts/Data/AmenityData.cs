using System;
using UnityEngine;

namespace ARDE.RealEstate3D.Data
{
    [Serializable]
    public class AmenityData
    {
        [SerializeField] private string title;
        [TextArea(1, 3)]
        [SerializeField] private string description;

        public string Title => title;
        public string Description => description;

        public AmenityData(string title, string description = "")
        {
            this.title = title;
            this.description = description;
        }
    }
}
