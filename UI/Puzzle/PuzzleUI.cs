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

        if (this._actionText == null)
        {
            this._actionText = this.transform.Find("ActionText")?.GetComponent<TextMeshProUGUI>();
        }        
        
        if (this._levelText == null)
        {
            this._levelText = this.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
        }       
        
        if (this._timeText == null)
        {
            this._timeText = this.transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        }

    }

    private void Update()
    {
        if (GameMode_Puzzle.Instance.IsGamePlaying())
        {
            this.UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        // action
        string newActionText = GameMode_Puzzle.Instance.GetGameActionCounter().ToString();
        if (newActionText != this._actionText.text)
        {
            this._actionText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                this._actionText.text = newActionText;
                this._actionText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad);
            });
        }

        // time
        int timeToInt = Mathf.CeilToInt(GameMode_Puzzle.Instance.GetGamePlayTimer());
        int minutes = timeToInt / 60;
        int seconds = timeToInt % 60;
        string newTimeText = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        if (newTimeText != this._timeText.text)
        {
            this._timeText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                this._timeText.text = newTimeText;
                this._timeText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad);
            });
        }

        // level
        int levelIndex = GameMode_Puzzle.Instance.GetLevelIndex() + 1;
        string newlevelText = this.IntToRoman(levelIndex).ToString();
        this._levelText.text = newlevelText;
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
        this.UpdateVisual();

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
