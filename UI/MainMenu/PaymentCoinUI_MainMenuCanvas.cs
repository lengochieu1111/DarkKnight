using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PaymentCoinUI_MainMenuCanvas : RyoMonoBehaviour
{
    [SerializeField, BoxGroup("PAYMENT")] private Button _exitButton;


    protected override void Awake()
    {
        base.Awake();
        
        _exitButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    

}
