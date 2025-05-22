using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HIEU_NL.ObjectPool.Profile
{
    public class AchivementSingleUI_LoginCanvas : RyoMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankingIndexText;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _currentLevelIndexText;
        [SerializeField] private TextMeshProUGUI _puzzleStateText;
        [SerializeField] private TextMeshProUGUI _weaponText;
        [SerializeField] private TextMeshProUGUI _characterText;

        private User _user;

        /*
         * 
         */

        public void UpdateVisual(int rankingIndex, User user)
        {
            _user = user;

            //##
            _rankingIndexText.text = rankingIndex.ToString();
            _nameText.text = user.Name;
            _currentLevelIndexText.text = (user.CurrentMaxLevelIndex + 1).ToString();
            _puzzleStateText.text = user.PuzzleUnlocked ? "Unlocked" : "Locked";
            _weaponText.text = user.Bag.Weapon.Count.ToString();
            _characterText.text = user.Bag.Character.Count.ToString();
        }
    }
}
