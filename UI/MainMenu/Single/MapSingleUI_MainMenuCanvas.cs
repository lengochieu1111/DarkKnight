using HIEU_NL.Manager;
using HIEU_NL.SO.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSingleUI_MainMenuCanvas : RyoMonoBehaviour
{
    [SerializeField] private Button _selectButton;

    [SerializeField] private Image _avatarImage;
    [SerializeField] private Image _lockImage;
    [SerializeField] private TextMeshProUGUI _mapIndexText;
    [SerializeField] private TextMeshProUGUI _mapNameText;
    private MapData _mapData;
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
        if (FirebaseManager.Instance.CurrentUser == null || !_isUnlocked) return;

        bool isPlayPuzzleScene = FirebaseManager.Instance.CurrentUser.CurrentMaxLevelIndex.Equals(_mapData.MapIndex) 
                                 && !FirebaseManager.Instance.CurrentUser.PuzzleUnlocked;
        
        FirebaseManager.Instance.UseLevel(_mapData.MapIndex);
            
        if (isPlayPuzzleScene)
        {
            SceneTransitionManager.Instance.LoadScene(EScene.Puzzle);
        }
        else
        {
            SceneTransitionManager.Instance.LoadScene(EScene.Platformer);
        }
    }
    
    public void UpdateVisual(MapData mapData, Sprite lockState, bool isUnlock)
    {
        _mapData = mapData;
        _isUnlocked = isUnlock;
        
        _avatarImage.sprite = mapData.MapSprite;
        _lockImage.sprite = lockState;
        
        _mapNameText.text = mapData.MapName;
        _mapIndexText.text = mapData.MapIndex.ToString();

    }

}
