using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewProfileUI_LoginCanvas : RyoMonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;

    [SerializeField] private Button _okButton;
    [SerializeField] private Button _exitButton;

    protected override void Awake()
    {
        base.Awake();

        _nameInputField.onValueChanged.AddListener((string inputText) =>
        {
            _nameInputField.text = inputText.Replace(" ", "");
            _nameInputField.ActivateInputField();
        });

        _okButton.onClick.AddListener(() =>
        {
            Ok();
        });

        _exitButton.onClick.AddListener(() =>
        {
            Exit();
        });

    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_nameInputField == null)
        {
            _nameInputField = transform.Find("NameInputField")?.GetComponent<TMP_InputField>();
        }

        if (_okButton == null)
        {
            _okButton = transform.Find("OkButton")?.GetComponent<Button>();
        }

        if (_exitButton == null)
        {
            _exitButton = transform.Find("ExitButton")?.GetComponent<Button>();
        }

    }

    /*
     * 
     */

    private async void Ok()
    {
        string newUserName = _nameInputField.text;

        if (string.IsNullOrEmpty(newUserName) ) 
        {
            Debug.Log("User name null or empty!");
        }
        else
        {
            bool result = await FirebaseManager.Instance.CreateNewUser(newUserName);

            if (result)
            {
                // TransitionManager.Instance.LoadMainMenuScene();

                Debug.Log("Create user success");
            }
            else
            {
                Debug.Log("Create user failed");
            }
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
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
