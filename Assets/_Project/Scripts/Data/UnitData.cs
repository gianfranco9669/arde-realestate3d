using System;
using UnityEngine;

namespace ARDE.RealEstate3D.Data
{
    [Serializable]
    public class UnitData
    {
        [SerializeField] private string code;
        [SerializeField] private string typology;
        [SerializeField] private string area;
        [SerializeField] private string status;
        [SerializeField] private string price;

        public string Code => code;
        public string Typology => typology;
        public string Area => area;
        public string Status => status;
        public string Price => price;

        public UnitData(string code, string typology, string area, string status, string price)
        {
            this.code = code;
            this.typology = typology;
            this.area = area;
            this.status = status;
            this.price = price;
        }
    }
}
