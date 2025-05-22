using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UI.MainMenu.Single;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MissionUI_MainMenuCanvas : RyoMonoBehaviour
{
    [SerializeField, BoxGroup] private MissionListSO _missionListSO;
    [SerializeField, BoxGroup] private ScrollRect _scrollView;
    [SerializeField, BoxGroup] private MissionSingleUI_MainMenuCanvas _missionSingleUIPrefab;
    [SerializeField, BoxGroup] private Button _exitButton;
    private List<MissionSingleUI_MainMenuCanvas> _missionUISingleList = new List<MissionSingleUI_MainMenuCanvas>();

    protected override void Awake()
    {
        base.Awake();
        
        _exitButton.onClick.AddListener(() =>
        {
            Hide();
        });

        CreateMissionSingleUI();
    }

    //#

    private void CreateMissionSingleUI()
    {
        foreach (MissionData missionData in _missionListSO.MissionDataList)
        {
            CreateNewMission(missionData);
        }
    }

    /*private void ClearVisual()
    {
        if (_mapSingleList.IsNullOrEmpty()) return;
        
        foreach (MapSingleUI_MainMenuCanvas mapSingle in _mapSingleList)
        {
            Destroy(mapSingle.gameObject);
        }
        
        _mapSingleList.Clear();
    }*/

    private void CreateNewMission(MissionData missionData)
    {
        MissionSingleUI_MainMenuCanvas missionSingleUI = Instantiate(_missionSingleUIPrefab, parent: _scrollView.content);

        if (missionSingleUI.IsValid())
        {

            missionSingleUI.UpdateVisual(missionData);
            /*bool isUnlocked = FirebaseManager.Instance.CurrentUser?.CurrentMaxLevelIndex >= mapData.MapIndex;
            Sprite lockState = isUnlocked ? _unlockSprite : _lockSprite;
            mapSingle.UpdateVisual(mapData, lockState, isUnlocked);

            _mapSingleList.Add(mapSingle);*/
            
            // FirebaseManager
            
            _missionUISingleList.Add(missionSingleUI);
        }
    }
    
    
    //#
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
