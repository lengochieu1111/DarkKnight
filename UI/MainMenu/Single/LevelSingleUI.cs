using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSingleUI : RyoMonoBehaviour
{
    [SerializeField] private Button _selectButton;

    [SerializeField] private TextMeshProUGUI _levelIndexText;

    protected override void Awake()
    {
        base.Awake();

        this._selectButton.onClick.AddListener(() =>
        {
            this.Select();
        });


    }

    private void Select()
    {
        Debug.Log("Select");
    }


}
