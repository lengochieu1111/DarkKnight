using System.Collections;
using System.Collections.Generic;
using HIEU_NL.SO.Character;
using HIEU_NL.SO.Weapon;
using HIEU_NL.Utilities;
using TMPro;
using UI.MainMenu.Single;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI_MainMenuCanvas : RyoMonoBehaviour
{
    [SerializeField] private WeaponDataListSO _weaponDataListSO;
    [SerializeField] private CharacterDataListSO _characterDataListSO;
    
    [SerializeField] private WeaponSingleUI_MainMenuCanvas _weaponSingleUIPrefab;        
    [SerializeField] private CharacterSingleUI_MainMenuCanvas _characterSingleUIPrefab;        
    
    [SerializeField] private ScrollRect _weaponScrollView;
    [SerializeField] private ScrollRect _characterScrollView;
    
    [SerializeField] private TextMeshProUGUI _selectName_Text;
    [SerializeField] private TextMeshProUGUI _selectCost_Text;
    [SerializeField] private Image _select_Image;
    [SerializeField] private TextMeshProUGUI _selectDetail_Text;
    
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _exitButton;
    private List<WeaponSingleUI_MainMenuCanvas> _weaponSingleList = new();
    private List<CharacterSingleUI_MainMenuCanvas> _characterSingleList = new();

    
    protected override void Awake()
    {
        base.Awake();

        _buyButton.onClick.AddListener(() => {
            Buy();
        });
        
        _exitButton.onClick.AddListener(() => {
            Exit();
        });

    }


    protected override void OnEnable()
    {
        base.OnEnable();
        //##
        WeaponSingleUI_MainMenuCanvas.OnSelectWeapon += WeaponSingleUI_OnSelectWeapon;
        CharacterSingleUI_MainMenuCanvas.OnSelectCharacter += CharacterSingleUI_OnSelectCharacter;
    }

    private void CharacterSingleUI_OnSelectCharacter(object sender, CharacterData e)
    {
        SelectCharacter(e);
    }

    private void WeaponSingleUI_OnSelectWeapon(object sender, WeaponData e)
    {
        SelectWeapon(e);
    }

    private void UpdateVisual()
    {
        foreach (WeaponData weaponData in _weaponDataListSO.WeaponDataList)
        {
            CreateWeapoList(weaponData);
        }
        
        foreach (CharacterData characterData in _characterDataListSO.CharacterDataList)
        {
            // CreateCharacterList(characterData);
        }
        
        //##
        void CreateWeapoList(WeaponData weaponData)
        {
            WeaponSingleUI_MainMenuCanvas weaponSingle = Instantiate(_weaponSingleUIPrefab, parent: _weaponScrollView.content);

            if (weaponSingle.IsValid())
            {
                weaponSingle.UpdateVisual(weaponData, false);
                _weaponSingleList.Add(weaponSingle);
            }
        }
    
        void CreateCharacterList(CharacterData characterData)
        {
            CharacterSingleUI_MainMenuCanvas characterSingle = Instantiate(_characterSingleUIPrefab, parent: _weaponScrollView.content);

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

    public void SelectWeapon(WeaponData weaponData)
    {
        _selectName_Text.text = weaponData.WeaponName;
        _selectCost_Text.text = weaponData.WeaponCost.ToString();
        _select_Image.sprite = weaponData.WeaponSprite;
        _selectDetail_Text.text = weaponData.WeaponDetail;
    }
    
    public void SelectCharacter(CharacterData characterData)
    {
        _selectName_Text.text = characterData.CharacterName;
        _selectCost_Text.text = characterData.CharacterCost.ToString();
        _select_Image.sprite = characterData.CharacterSprite;
        _selectDetail_Text.text = characterData.CharacterDetail;
    }

    
    /*
     * Button Function
     */
    
    private void Buy()
    {
        Debug.Log("Buy : ");
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
