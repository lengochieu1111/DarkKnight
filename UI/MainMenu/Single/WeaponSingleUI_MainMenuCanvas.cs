using System;
using HIEU_NL.SO.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Single
{
    public class WeaponSingleUI_MainMenuCanvas : RyoMonoBehaviour
    {
        public static event EventHandler<WeaponData> OnSelectWeapon;

        [SerializeField] private Button _selectButton;

        [SerializeField] private Image _weaponImage;
        private WeaponData _weaponData;
        private bool _isUnlocked;

        protected override void Awake()
        {
            base.Awake();

            //##
            _selectButton.onClick.AddListener(() =>
            {
                Select();
            });

        }

        private void Select()
        {
            // Unlock or Lock
            OnSelectWeapon?.Invoke(this, _weaponData);
        }
    
        public void UpdateVisual(WeaponData weaponData, bool isUnlock)
        {
            _weaponData = weaponData;
            _isUnlocked = isUnlock;
            
            _weaponImage.sprite = weaponData.WeaponSprite;
        }
    }
}