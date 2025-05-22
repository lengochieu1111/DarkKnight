using System.Collections;
using System.Collections.Generic;
using HIEU_NL.SO.Character;
using HIEU_NL.SO.Weapon;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using TMPro;
using UI.MainMenu.Single;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI_MainMenuCanvas : RyoMonoBehaviour
{
    private const string SelectAction_Buy = "Buy";
    private const string SelectAction_Use = "Use";
    private const string SelectAction_Used = "Used";
    
    [SerializeField] private WeaponDataListSO _weaponDataListSO;
    [SerializeField] private CharacterDataListSO _characterDataListSO;
    
    [SerializeField] private WeaponSingleUI_MainMenuCanvas _weaponSingleUIPrefab;        
    [SerializeField] private CharacterSingleUI_MainMenuCanvas _characterSingleUIPrefab;        
    
    [SerializeField] private ScrollRect _weaponScrollView;
    [SerializeField] private ScrollRect _characterScrollView;
    
    [SerializeField] private TextMeshProUGUI _selectName_Text;
    [SerializeField] private TextMeshProUGUI _selectCost_Text;
    [SerializeField] private TextMeshProUGUI _selectTitleCost_Text;
    
    [SerializeField] private Image _selectWeapon_Image;
    [SerializeField] private TextMeshProUGUI _selectWeaponDetail_Text;
    
    [SerializeField] private Image _selectCharacter_Image;
    [SerializeField] private TextMeshProUGUI _selectCharacterDetail_Text;
    
    [SerializeField] private Button _selectWeapon_Button;
    [SerializeField] private TextMeshProUGUI _selectWeaponButton_Text;
    
    [SerializeField] private Button _selectCharacter_Button;
    [SerializeField] private TextMeshProUGUI _selectCharacterButton_Text;
    
    [SerializeField] private Button _exitButton;
    private List<WeaponSingleUI_MainMenuCanvas> _weaponSingleList = new();
    private List<CharacterSingleUI_MainMenuCanvas> _characterSingleList = new();
    private bool _isWeaponUnlocked;
    private bool _isCharacterUnlocked;
    private WeaponData _currentWeaponData;
    private CharacterData _currentCharacterData;
    
    
    [SerializeField, BoxGroup("PAYMENT")] private PaymentCoinUI_MainMenuCanvas _paymentObject;
    [SerializeField, BoxGroup("PAYMENT")] private TextMeshProUGUI _coinText;
    [SerializeField, BoxGroup("PAYMENT")] private Button _addCoinButton;
    [SerializeField, BoxGroup("PAYMENT")] private Button _adsButton;
    
    
    protected override void Awake()
    {
        base.Awake();

        _selectWeapon_Button.onClick.AddListener(() => {
            SelectWeaponAction();
        });
        
        _selectCharacter_Button.onClick.AddListener(() => {
            SelectCharacterAction();
        });
        
        _exitButton.onClick.AddListener(() => {
            Exit();
        });
        
        _addCoinButton.onClick.AddListener(() =>
        {
            ShowPaymentUI();
        });

    }


    protected override void OnEnable()
    {
        base.OnEnable();
        //##

        _selectName_Text.text = "";
        _selectCost_Text.text = "";
        _selectTitleCost_Text.text = "";
            
        HideSelectWeaponButton();
        HideSelectCharacterButton();
        HideSelectWeapon_Image_Detail();
        HideSelectCharacter_Image_Detail();

        int currentSelectedWeapon = FirebaseManager.Instance.CurrentUser.CurrentWeaponIndex;
        SelectWeapon(_weaponDataListSO.WeaponDataList[currentSelectedWeapon], true);
        
        WeaponSingleUI_MainMenuCanvas.OnSelectWeapon += WeaponSingleUI_OnSelectWeapon;
        CharacterSingleUI_MainMenuCanvas.OnSelectCharacter += CharacterSingleUI_OnSelectCharacter;
        
        PaymentCoinSingleUI_MainMenuCanvas.OnPayment += PaymentCoinUI_OnPayment;
    }

    private void PaymentCoinUI_OnPayment(object sender, int e)
    {
        FirebaseManager.Instance.PaymentCoin(e);
        
        UpdateVisualCoin();
    }

    private void CharacterSingleUI_OnSelectCharacter(object sender, CharacterData e)
    {
        _currentCharacterData = e;
        SelectCharacter(e);
    }

    private void WeaponSingleUI_OnSelectWeapon(object sender, WeaponData e)
    {
        _currentWeaponData = e;
        SelectWeapon(e);
    }

    private void UpdateVisual()
    {
        UpdateVisualCoin();
        
        foreach (WeaponData weaponData in _weaponDataListSO.WeaponDataList)
        {
            CreateWeapoList(weaponData);
        }
        
        foreach (CharacterData characterData in _characterDataListSO.CharacterDataList)
        {
            CreateCharacterList(characterData);
        }
        
        //##
        void CreateWeapoList(WeaponData weaponData)
        {
            WeaponSingleUI_MainMenuCanvas weaponSingle = Instantiate(_weaponSingleUIPrefab, parent: _weaponScrollView.content);
            weaponSingle.gameObject.name = weaponData.WeaponName;

            if (weaponSingle.IsValid())
            {
                weaponSingle.UpdateVisual(weaponData, false);
                _weaponSingleList.Add(weaponSingle);
            }
        }
    
        void CreateCharacterList(CharacterData characterData)
        {
            CharacterSingleUI_MainMenuCanvas characterSingle = Instantiate(_characterSingleUIPrefab, parent: _characterScrollView.content);
            characterSingle.gameObject.name = characterData.CharacterName;

            if (characterSingle.IsValid())
            {
                characterSingle.UpdateVisual(characterData, false);
                _characterSingleList.Add(characterSingle);
            }
        }
        
    }

    private void ClearVisual()
    {
        foreach (WeaponSingleUI_MainMenuCanvas weaponSingle in _weaponSingleList)
        {
            Destroy(weaponSingle.gameObject);
        }
        
        foreach (CharacterSingleUI_MainMenuCanvas characterSingle in _characterSingleList)
        {
            Destroy(characterSingle.gameObject);
        }
        
        _weaponSingleList.Clear();
        _characterSingleList.Clear();
    }

    public void SelectWeapon(WeaponData weaponData, bool isStartShowShop = false)
    {
        _selectTitleCost_Text.text = "Cost : ";
            
        _selectName_Text.text = weaponData.WeaponName;
        _selectCost_Text.text = weaponData.WeaponCost.ToString();
        _selectWeapon_Image.sprite = weaponData.WeaponSprite;
        _selectWeaponDetail_Text.text = weaponData.WeaponDetail;
        
        ShowSelectWeapon_Image_Detail();
        HideSelectCharacter_Image_Detail();
        
        if (isStartShowShop)
        {
            if (FirebaseManager.Instance.CurrentUser.CurrentWeaponIndex.Equals(weaponData.WeaponIndex))
            {
                _isWeaponUnlocked = true;
                _selectWeaponButton_Text.text = SelectAction_Used;
            }
            else
            {
                _isWeaponUnlocked = true;
                _selectWeaponButton_Text.text = SelectAction_Use;
            }
        }
        else if (FirebaseManager.Instance.CurrentUser.Bag.Weapon.Contains(weaponData.WeaponIndex))
        {
            _isWeaponUnlocked = true;
            _selectWeaponButton_Text.text = SelectAction_Use;
        }
        else
        {
            _isWeaponUnlocked = false;
            _selectWeaponButton_Text.text = SelectAction_Buy;
        }
        
        ShowSelectWeaponButton();
        HideSelectCharacterButton();
    }
    
    public void SelectCharacter(CharacterData characterData)
    {
        _selectTitleCost_Text.text = "Cost : ";

        _selectName_Text.text = characterData.CharacterName;
        _selectCost_Text.text = characterData.CharacterCost.ToString();
        _selectCharacter_Image.sprite = characterData.CharacterSprite;
        _selectCharacterDetail_Text.text = characterData.CharacterDetail;
        
        ShowSelectCharacter_Image_Detail();
        HideSelectWeapon_Image_Detail();
        
        if (FirebaseManager.Instance.CurrentUser.Bag.Character.Contains(characterData.CharacterIndex))
        {
            _isCharacterUnlocked = true;
            _selectCharacterButton_Text.text = SelectAction_Use;
        }
        else
        {
            _isCharacterUnlocked = false;
            _selectCharacterButton_Text.text = SelectAction_Buy;
        }
        
        ShowSelectCharacterButton();
        HideSelectWeaponButton();
    }

    
    /*
     * Button Function
     */
    
    private void SelectWeaponAction()
    {
        if (_isWeaponUnlocked)
        {
            Debug.Log("Use Weapon");
            _selectWeaponButton_Text.text = SelectAction_Used;
            FirebaseManager.Instance.UseWeapon(_currentWeaponData.WeaponIndex);
        }
        else if (FirebaseManager.Instance.CurrentUser.CurrentCoin >= _currentWeaponData.WeaponCost)
        {
            Debug.Log("Buy Weapon");
            _isWeaponUnlocked = true;
            _selectWeaponButton_Text.text = SelectAction_Use;
            FirebaseManager.Instance.BuyWeapon(_currentWeaponData.WeaponIndex, _currentWeaponData.WeaponCost);
            FirebaseManager.Instance.UpdateMissionAmount(2, 1);

            UpdateVisualCoin();
        }
    }
    
    private void SelectCharacterAction()
    {
        if (_isCharacterUnlocked)
        {
            Debug.Log("Use Character");
            _selectCharacterButton_Text.text = SelectAction_Used;
            FirebaseManager.Instance.UseCharacter(_currentCharacterData.CharacterIndex);
        }
        else if (FirebaseManager.Instance.CurrentUser.CurrentCoin >= _currentCharacterData.CharacterCost)
        {
            Debug.Log("Buy Character");
            _isCharacterUnlocked = true;
            _selectCharacterButton_Text.text = SelectAction_Use;
            FirebaseManager.Instance.BuyCharacter(_currentCharacterData.CharacterIndex, _currentCharacterData.CharacterCost);
            
            UpdateVisualCoin();
        } 
    }
    
    private void Exit()
    {
        Hide();
    }

    private void ShowPaymentUI()
    {
        _paymentObject.Show();
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
    
    public void ShowSelectWeaponButton()
    {
        _selectWeapon_Button.gameObject.SetActive(true);
    }

    public void HideSelectWeaponButton()
    {
        _selectWeapon_Button.gameObject.SetActive(false);
    }
    
    public void ShowSelectCharacterButton()
    {
        _selectCharacter_Button.gameObject.SetActive(true);
    }

    public void HideSelectCharacterButton()
    {
        _selectCharacter_Button.gameObject.SetActive(false);
    }
    
    public void ShowSelectCharacter_Image_Detail()
    {
        _selectCharacterDetail_Text.gameObject.SetActive(true);
        _selectCharacter_Image.gameObject.SetActive(true);
    }

    public void HideSelectCharacter_Image_Detail()
    {
        _selectCharacterDetail_Text.gameObject.SetActive(false);
        _selectCharacter_Image.gameObject.SetActive(false);
    }
    
    public void ShowSelectWeapon_Image_Detail()
    {
        _selectWeaponDetail_Text.gameObject.SetActive(true);
        _selectWeapon_Image.gameObject.SetActive(true);
    }

    public void HideSelectWeapon_Image_Detail()
    {
        _selectWeaponDetail_Text.gameObject.SetActive(false);
        _selectWeapon_Image.gameObject.SetActive(false);
    }

    private void UpdateVisualCoin()
    {
        _coinText.text = FirebaseManager.Instance.CurrentUser.CurrentCoin.ToString();
    }

}
