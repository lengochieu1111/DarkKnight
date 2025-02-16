using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Game;
using TMPro;
using UnityEngine;

public class PuzzleUI : RyoMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _timeText;

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_actionText == null)
        {
            _actionText = transform.Find("ActionText")?.GetComponent<TextMeshProUGUI>();
        }        
        
        if (_levelText == null)
        {
            _levelText = transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
        }       
        
        if (_timeText == null)
        {
            _timeText = transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        }

    }

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
        string newActionText = GameMode_Puzzle.Instance.GetGameActionCounter().ToString();
        if (newActionText != _actionText.text)
        {
            _actionText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _actionText.text = newActionText;
                _actionText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad);
            });
        }

        // time
        int timeToInt = Mathf.CeilToInt(GameMode_Puzzle.Instance.GetGamePlayTimer());
        int minutes = timeToInt / 60;
        int seconds = timeToInt % 60;
        string newTimeText = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        if (newTimeText != _timeText.text)
        {
            _timeText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _timeText.text = newTimeText;
                _timeText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad);
            });
        }

        // level
        int levelIndex = GameMode_Puzzle.Instance.GetLevelIndex() + 1;
        string newlevelText = IntToRoman(levelIndex).ToString();
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
