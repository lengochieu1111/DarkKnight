using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PaymentCoinSingleUI_MainMenuCanvas : MonoBehaviour
{
    public static event EventHandler<int> OnPayment;
    
    [SerializeField, BoxGroup] private Button _button;
    [SerializeField, BoxGroup] private int _cointAmount;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            OnPayment?.Invoke(this, _cointAmount);
        });
    }
    
}
