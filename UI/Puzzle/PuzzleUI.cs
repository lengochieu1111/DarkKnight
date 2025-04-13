using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Game;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PuzzleUI : RyoMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _timeText;
    
    [HorizontalLine]
    [SerializeField] private float _actionZoomInScale = 1.2f;
    [SerializeField] private float _actionZoomOutScale = 1f;
    [SerializeField] private float _actionZoomInDuration = 0.1f;
    [SerializeField] private float _actionZoomOutDuration = 0.2f;
    
    [HorizontalLine]
    [SerializeField] private float _timeTextZoomInScale = 1.2f;
    [SerializeField] private float _timeTextZoomOutScale = 1f;
    [SerializeField] private float _timeTextZoomInDuration = 0.3f;
    [SerializeField] private float _timeTextZoomOutDuration = 0.4f;
    
    private void Update()
    {
        if (GameMode_Puzzle.Instance.IsGamePlaying())
        {
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        // action
        string newActionText;
        bool isEndOfActionCount = GameMode_Puzzle.Instance.GetGameActionCounter() <= 0;
        if (isEndOfActionCount)
        {
            newActionText = "X";
        }
        else
        {
            newActionText = GameMode_Puzzle.Instance.GetGameActionCounter().ToString();
        }

        if ((GameMode_Puzzle.Instance.Player.IsValid() && GameMode_Puzzle.Instance.Player.IsPain())
            || isEndOfActionCount)
        {
            _actionText.color = Color.red;
        }
        else
        {
            _actionText.color = Color.white;
        }
        
        if (newActionText != _actionText.text)
        {
            _actionText.transform.DOScale(_actionZoomInScale, _actionZoomInDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _actionText.text = newActionText;
                _actionText.transform.DOScale(_actionZoomOutScale, _actionZoomOutDuration).SetEase(Ease.OutQuad);
            });
        }

        // time
        int timeToInt = Mathf.CeilToInt(GameMode_Puzzle.Instance.GetGamePlayTimer());
        int minutes = timeToInt / 60;
        int seconds = timeToInt % 60;
        string newTimeText = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        if (newTimeText != _timeText.text)
        {
            _timeText.transform.DOScale(_timeTextZoomInScale, _timeTextZoomInDuration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _timeText.text = newTimeText;
                _timeText.transform.DOScale(_timeTextZoomOutScale, _timeTextZoomOutDuration).SetEase(Ease.OutQuad);
            });
        }

        // level
        int levelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex + 1;
        string newlevelText = IntToRoman(levelIndex);
        _levelText.text = newlevelText;
    }

    public string IntToRoman(int number)
    {
        if (number < 1 || number > 100)
        {
            return "Invalid";
        }

        string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
        string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };

        string roman = tens[number / 10] + ones[number % 10];

        return roman;
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
        gameObject.SetActive(false);
    }

}
