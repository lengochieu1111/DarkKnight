using System.Collections;
using System.Collections.Generic;
using HIEU_NL.SO.Map;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class SelectMapUI_MainMenuCanvas : RyoMonoBehaviour
{
    [SerializeField] private MapDataListSO _mapDataListSO;
    
    [SerializeField] private MapSingleUI_MainMenuCanvas _mapSingleUIPrefab;        
    
    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;
    
    [BoxGroup, SerializeField] private Sprite _lockSprite;
    [BoxGroup, SerializeField] private Sprite _unlockSprite;

    private List<MapSingleUI_MainMenuCanvas> _mapSingleList = new();

    protected override void Awake()
    {
        base.Awake();

        _exitButton.onClick.AddListener(() => {
            Exit();
        });

    }

    /*
     * 
     */

    private void UpdateVisual()
    {
        foreach (MapData mapData in _mapDataListSO.MapAssetList)
        {
            CreateNewMap(mapData);
        }
    }

    private void ClearVisual()
    {
        foreach (MapSingleUI_MainMenuCanvas mapSingle in _mapSingleList)
        {
            Destroy(mapSingle.gameObject);
        }
        
        _mapSingleList.Clear();
    }

    private void CreateNewMap(MapData mapData)
    {
        MapSingleUI_MainMenuCanvas mapSingle = Instantiate(_mapSingleUIPrefab, parent: _scrollView.content);

        if (mapSingle.IsValid())
        {
            bool isUnlocked = FirebaseManager.Instance.CurrentUser?.CurrentLevelIndex >= mapData.MapIndex;
            Sprite lockState = isUnlocked ? _unlockSprite : _lockSprite;
            mapSingle.UpdateVisual(mapData, lockState, isUnlocked);

            _mapSingleList.Add(mapSingle);
        }
    }

    private void Exit()
    {
        Hide();
    }

    /*
     * 
     */

    public void Show()
    {
        UpdateVisual();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClearVisual();

        gameObject.SetActive(false);
    }

}
