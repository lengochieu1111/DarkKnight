using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HIEU_NL.ObjectPool.Profile
{
    using DesignPatterns.ObjectPool.Multiple;

    public class ProfilePrefab : PoolPrefab
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

            this._selectButton.onClick.AddListener(() =>
            {
                this.Select();
            });

            this._clearButton.onClick.AddListener(() =>
            {
                this.Clear();
            });

        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (this._selectButton == null)
            {
                this._selectButton = this.transform.Find("SelectButton")?.GetComponent<Button>();
            }

            if (this._clearButton == null)
            {
                this._clearButton = this.transform.Find("ClearButton")?.GetComponent<Button>();
            }

            //

            if (this._rankingIndexText == null)
            {
                this._rankingIndexText = this.transform.Find("RankingIndexText")?.GetComponent<TextMeshProUGUI>();
            }

            if (this._avatarImage == null)
            {
                this._avatarImage = this.transform.Find("AvatarImage")?.GetComponent<Image>();
            }

            if (this._nameText == null)
            {
                this._nameText = this.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            }

            if (this._currentLevelIndexText == null)
            {
                this._currentLevelIndexText = this.transform.Find("CurrentLevelIndexText")?.GetComponent<TextMeshProUGUI>();
            }

        }

        /*
         * 
         */

        private void Select()
        {
            FirebaseManager.Instance.RequestLoginUser(this._user);
        }

        private async void Clear()
        {
            bool result = await FirebaseManager.Instance.DeleteUser(this._user);

            if (result)
            {
                this.Deactivate();
            }

        }

        public void UpdateVisual(int rankingIndex, User user)
        {
            this._user = user;

            //#
            this._rankingIndexText.text = rankingIndex.ToString();
            this._nameText.text = user.Name;
            this._currentLevelIndexText.text = user.CurrentLevelIndex.ToString();

        }

    }

}
