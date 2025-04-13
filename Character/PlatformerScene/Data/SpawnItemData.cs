using System;
using HIEU_NL.Platformer.Script.GameItem;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.SerializableClass
{
    [Serializable]
    public class SpawnItemData
    {
        [field: SerializeField] public bool HasMedicineHealth { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineHealth"), MinMaxSlider(0f, 1f)] public Vector2 MedicineHealth_Thirty_QuantityRange { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineHealth"), MinMaxSlider(0f, 1f)] public Vector2 MedicineHealth_Seventy_QuantityRange { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineHealth"), MinMaxSlider(0f, 1f)] public Vector2 MedicineHealth_Hundred_QuantityRange { get; private set; }
        
        [field: SerializeField] public bool HasMedicineEnergy { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineEnergy"), MinMaxSlider(0f, 1f)] public Vector2 MedicineEnergy_Thirty_QuantityRange { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineEnergy"), MinMaxSlider(0f, 1f)] public Vector2 MedicineEnergy_Seventy_QuantityRange { get; private set; }
        [field: SerializeField, ShowIf("HasMedicineEnergy"), MinMaxSlider(0f, 1f)] public Vector2 MedicineEnergy_Hundred_QuantityRange { get; private set; }
    }
}