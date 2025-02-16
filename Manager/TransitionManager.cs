using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using static HIEU_NL.Utilities.ParameterExtensions.Window;

public enum Scene
{
    Intro,
    Login,
    Preparation,
    MainMenu,
    Puzzle,
    Platformer
}

public class TransitionManager : PersistentSingleton<TransitionManager>
{
    /*public event EventHandler LoadingSceneBegin;
    public event EventHandler LoadingSceneFinish;*/
    
    public enum TransitionType
    {
        Fade,
        Fast
    }

    [SerializeField] private Scene _scene;
    [SerializeField] private Image _fadeTransitionImage;
    [SerializeField] private Transform _fastTransitionTransform;
    [SerializeField] private float _sceneTransitionTime = 1f;

    protected override void Start()
    {
        base.Start();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_fadeTransitionImage == null)
        {
            _fadeTransitionImage = GetComponentInChildren<Image>(true);
        }

    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        HideFadeTransition();

    }

    /*
     * 
     */

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        _fadeTransitionImage.DOFade(0, _sceneTransitionTime).OnComplete(() =>
        {
            HideFadeTransition();

            // LoadingSceneFinish?.Invoke(this, EventArgs.Empty);

        });
    }

    /*
     * 
     */

    public void LoadScene(Scene scene, bool useLoadCallback = false, TransitionType transitionType = TransitionType.Fade)
    {
        _scene = scene;

        if (transitionType is TransitionType.Fade)
        {
            ShowFadeTransition();
        
            _fadeTransitionImage.DOFade(1, _sceneTransitionTime).OnComplete(() =>
            {
                // LoadingSceneBegin?.Invoke(this, EventArgs.Empty);

                Load();

            });
        }
        else
        {
            ShowFastTransition();
            
            _fastTransitionTransform.DOMoveY(WINDOW_HEIGHT / 2, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Load();
            });
            
        }

        void Load()
        {
            if (useLoadCallback)
            {
                SceneManager.LoadScene((int)Scene.Preparation);
            }
            else
            {
                SceneManager.LoadScene((int)_scene);
            }
        }

    }
    
    public void LoadCallback()
    {
        SceneManager.LoadScene((int)_scene);
    }

    /*
     * 
     */

    private void ShowFadeTransition()
    {
        _fadeTransitionImage.gameObject.SetActive(true);
    }
    
    private void HideFadeTransition()
    {
        _fadeTransitionImage.DOFade(0, 0);
        _fadeTransitionImage.gameObject.SetActive(false);
    }

    /*
     *
     */

    public void ShowFastTransition()
    {
        _fastTransitionTransform.DOMoveY(-WINDOW_HEIGHT, 0);
        _fastTransitionTransform.gameObject.SetActive(true);
    }

    public void HideFastTransition()
    {
        _fastTransitionTransform.DOMoveY(-WINDOW_HEIGHT, 0);
        _fastTransitionTransform.gameObject.SetActive(false);
    }

}
