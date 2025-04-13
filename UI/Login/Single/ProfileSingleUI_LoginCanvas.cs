using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HIEU_NL.ObjectPool.Profile
{
    public class ProfileSingleUI_LoginCanvas : RyoMonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _clearButton;

        [SerializeField] private TextMeshProUGUI _rankingIndexText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _currentLevelIndexText;

        private User _user;

        protected override void Awake()
        {
            base.Awake();

            _selectButton.onClick.AddListener(() =>
            {
                Select();
            });

            _clearButton.onClick.AddListener(() =>
            {
                Clear();
            });

        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (_selectButton == null)
            {
                _selectButton = transform.Find("SelectButton")?.GetComponent<Button>();
            }

            if (_clearButton == null)
            {
                _clearButton = transform.Find("ClearButton")?.GetComponent<Button>();
            }

            //

            if (_rankingIndexText == null)
            {
                _rankingIndexText = transform.Find("RankingIndexText")?.GetComponent<TextMeshProUGUI>();
            }

            if (_avatarImage == null)
            {
                _avatarImage = transform.Find("AvatarImage")?.GetComponent<Image>();
            }

            if (_nameText == null)
            {
                _nameText = transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            }

            if (_currentLevelIndexText == null)
            {
                _currentLevelIndexText = transform.Find("CurrentLevelIndexText")?.GetComponent<TextMeshProUGUI>();
            }

        }

        /*
         * 
         */

        private void Select()
        {
            FirebaseManager.Instance.RequestLoginUser(_user);
        }

        private async void Clear()
        {
            bool result = await FirebaseManager.Instance.DeleteUser(_user);

            if (result)
            {
                Destroy(gameObject);
            }

        }

        public void UpdateVisual(int rankingIndex, User user)
        {
            _user = user;

            //##
            _rankingIndexText.text = rankingIndex.ToString();
            _nameText.text = user.Name;
            _currentLevelIndexText.text = user.CurrentMaxLevelIndex.ToString();

        }

    }

}
