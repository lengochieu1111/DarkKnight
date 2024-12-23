using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public enum Scene
{
    DataLoading,
    Login,
    Preparation,
    MainMenu,
    Puzzle,
    Platformer
}

public class TransitionManager : PersistentSingleton<TransitionManager>
{

    public event EventHandler LoadingSceneBegin;
    public event EventHandler LoadingSceneFinish;

    [SerializeField] private Scene _scene;
    [SerializeField] private Image _transitionImage;


    protected override void Start()
    {
        base.Start();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_transitionImage == null)
        {
            _transitionImage = GetComponentInChildren<Image>(true);
        }

    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        HideTransitionImage();

    }

    /*
     * 
     */

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        _transitionImage.DOFade(0, 1f).OnComplete(() =>
        {
            HideTransitionImage();

            LoadingSceneFinish?.Invoke(this, EventArgs.Empty);

        });
    }

    /*
     * 
     */

    public void Load_LoginScene()
    {
        Load(Scene.Login);
    }
    
    public void Load_MainMenuScene()
    {
        Load(Scene.MainMenu);
    }

    public void Load_PuzzleScene()
    {
        Load(Scene.Puzzle);
    }

    public void Load_PlatformerScene()
    {
        Load(Scene.Platformer);
    }

    private void Load(Scene scene)
    {
        _scene = scene;

        ShowTransitionImage();
        
        _transitionImage.DOFade(1, 1f).OnComplete(() =>
        {
            LoadingSceneBegin?.Invoke(this, EventArgs.Empty);

            // SceneManager.LoadScene((int)Scene.Preparation);

            SceneManager.LoadScene((int)_scene);

        });

    }

    public void LoadCallback()
    {
        SceneManager.LoadScene((int)_scene);
    }

    /*
     * 
     */

    private void ShowTransitionImage()
    {
        _transitionImage.gameObject.SetActive(true);
    }
    
    private void HideTransitionImage()
    {
        _transitionImage.DOFade(0, 0);

        _transitionImage.gameObject.SetActive(false);
    }

}
