using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewProfileUI : RyoMonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;

    [SerializeField] private Button _okButton;
    [SerializeField] private Button _exitButton;

    protected override void Awake()
    {
        base.Awake();

        this._nameInputField.onValueChanged.AddListener((string inputText) =>
        {
            this._nameInputField.text = inputText.Replace(" ", "");
            this._nameInputField.ActivateInputField();
        });

        this._okButton.onClick.AddListener(() =>
        {
            this.Ok();
        });

        this._exitButton.onClick.AddListener(() =>
        {
            this.Exit();
        });

    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._nameInputField == null)
        {
            this._nameInputField = this.transform.Find("NameInputField")?.GetComponent<TMP_InputField>();
        }

        if (this._okButton == null)
        {
            this._okButton = this.transform.Find("OkButton")?.GetComponent<Button>();
        }

        if (this._exitButton == null)
        {
            this._exitButton = this.transform.Find("ExitButton")?.GetComponent<Button>();
        }

    }

    /*
     * 
     */

    private async void Ok()
    {
        string newUserName = this._nameInputField.text;

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
        this.Hide();
    }

    /*
     * 
     */

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
