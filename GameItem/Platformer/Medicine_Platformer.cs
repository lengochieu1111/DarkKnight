using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.GameItem
{
    public class Medicine_Platformer : BaseGameItem_Platformer
    {
        public enum EMedicineType
        {
            Health,
            Energy
        }
        
        public enum ECapacityType
        {
            Thirty = 30,
            Seventy = 70,
            Hundred = 100,
        }

        [field: SerializeField, BoxGroup] public EMedicineType MedicineType { get; private set; }
        [field: SerializeField, BoxGroup] public ECapacityType CapacityType { get; private set; }
        

    }
}

