using System;
using HIEU_NL.SO.Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Single
{
    public class CharacterSingleUI_MainMenuCanvas : RyoMonoBehaviour
    {
        public static event EventHandler<CharacterData> OnSelectCharacter;

        [SerializeField] private Button _selectButton;

        [SerializeField] private Image _characterDataImage;
        private CharacterData _characterData;
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
            OnSelectCharacter?.Invoke(this, _characterData);
            // Unlock or Lock
        }
    
        public void UpdateVisual(CharacterData characterData, bool isUnlock)
        {
            _characterData = characterData;
            _isUnlocked = isUnlock;
            
            _characterDataImage.sprite = characterData.CharacterSprite;

        }
    }
}