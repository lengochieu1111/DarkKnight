using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Single
{
    public class MissionSingleUI_MainMenuCanvas : RyoMonoBehaviour
    {
        
        private const string STATE_NotAccepted = "Not Accepted";
        private const string STATE_Reveive = "Reveive";
        private const string STATE_Reveived = "Reveived";
        
        [SerializeField] private Button _receiveButton;
        [SerializeField] private Image _missionImage;
        [SerializeField] private TextMeshProUGUI _missionText;
        [SerializeField] private TextMeshProUGUI _rewardCoinText;
        [SerializeField] private TextMeshProUGUI _stateMissionText;
        private MissionData _missionData;

        protected override void Awake()
        {
            base.Awake();

            //##
            _receiveButton.onClick.AddListener(() =>
            {
                Receive();
            });

        }


        private void Receive()
        {
            if (FirebaseManager.Instance.CurrentUser.Mission.State[_missionData.MissionIndex]) return;
                
            if (FirebaseManager.Instance.CurrentUser.Mission.Amount[_missionData.MissionIndex] >=
                _missionData.MissionAmmount)
            {
                FirebaseManager.Instance.UpdateMissionState(_missionData.MissionIndex, _missionData.RewardCoin);
            }

            UpdateVisualState();
        }

        public void UpdateVisual(MissionData missionData)
        {
            _missionData = missionData;

            _missionImage.sprite = missionData.MissionSprite;
            _missionText.text = missionData.MissionText;
            _rewardCoinText.text = missionData.RewardCoin.ToString();
            
            UpdateVisualState();
            
            gameObject.SetActive(true);
        }

        private void UpdateVisualState()
        {
            if (FirebaseManager.Instance.CurrentUser.Mission.State[_missionData.MissionIndex])
            {
                _stateMissionText.text = STATE_Reveived;
            }
            else
            {
                if (FirebaseManager.Instance.CurrentUser.Mission.Amount[_missionData.MissionIndex] >=
                    _missionData.MissionAmmount)
                {
                    _stateMissionText.text = STATE_Reveive;
                }
                else
                {
                    _stateMissionText.text = STATE_NotAccepted;
                }
            }
        }
        
        
    }
}