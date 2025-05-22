using System.Collections;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using DG.Tweening;
using UnityEngine.SceneManagement;
using static HIEU_NL.Utilities.ParameterExtensions.Window;

namespace HIEU_NL.Manager
{
    public enum EScene
    {
        Intro,
        Login,
        MainMenu,
        Puzzle,
        Platformer,
        
        Preparation
    }

    public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
    {
        [SerializeField] private EScene _eScene;
        [SerializeField] private RectTransform _transitionImageRectTransform;
        
        [Header("Transition Settings")]
        [SerializeField] private float _transitionDuration = 0.5f;
        [SerializeField] private Ease _inEase = Ease.OutQuint;
        [SerializeField] private Ease _outEase = Ease.InQuint;
        [SerializeField] private float _delayBetweenTransitions = 0.2f;
        
        [Header("Helltaker Style Effects")]
        [SerializeField] private bool _useHeavyImpact = true;
        [SerializeField] private float _cameraShakeStrength = 0.2f;
        [SerializeField] private int _cameraShakeVibrato = 10;
        [SerializeField] private float _impactPauseTime = 0.1f;
        [SerializeField] private AudioClip _transitionPart1SoundEffect;
        [SerializeField] private AudioClip _transitionPart2SoundEffect;
        
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Camera _mainCamera;
        private bool _isTransitioning = false;

        protected override void Awake()
        {
            base.Awake();
            
            // Initially hide the transition image
            HideTransitionImage();
        }

        protected override void Start()
        {
            base.Start();
            
            // Find main camera for shake effect
            _mainCamera = Camera.main;
        }
        
        /// <summary>
        /// Load a scene with Helltaker-style transition
        /// </summary>
        public void LoadScene(EScene eScene)
        {
            if (_isTransitioning) return;
            
            _eScene = eScene;
            StartCoroutine(TransitionCoroutine());
        }
        
        /// <summary>
        /// Reload the current scene with Helltaker-style transition
        /// </summary>
        public void ReloadCurrentScene()
        {
            if (_isTransitioning) return;
            
            _eScene = (EScene)SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(TransitionCoroutine(true));
        }
        
        private void ShowTransitionImage()
        {
            // Position below screen
            _transitionImageRectTransform.anchoredPosition = new Vector2(0, -WindowHeight);
            _transitionImageRectTransform.gameObject.SetActive(true);
        }

        private void HideTransitionImage()
        {
            _transitionImageRectTransform.gameObject.SetActive(false);
        }
        
        private IEnumerator TransitionCoroutine(bool isReloadScene = false)
        {
            _isTransitioning = true;

            ShowTransitionImage();
            
            Tween transitionIn = _transitionImageRectTransform.DOAnchorPosY(0, _transitionDuration)
                .SetEase(_inEase);
            
            if (_audioSource != null && _transitionPart1SoundEffect != null)
            {
                _audioSource.clip = _transitionPart1SoundEffect;
                _audioSource.Play();
            }
            
            yield return transitionIn.WaitForCompletion();
            
            _mainCamera = Camera.main;
            
            if (_useHeavyImpact && _mainCamera != null)
            {
                _mainCamera.DOShakePosition(_impactPauseTime, _cameraShakeStrength, _cameraShakeVibrato);
                
                yield return new WaitForSeconds(_impactPauseTime);
            }
            
            // Step 2: Load the new scene
            if (isReloadScene)
            {
                AsyncOperation asyncLoadPre = SceneManager.LoadSceneAsync((int)EScene.Preparation);
                asyncLoadPre.allowSceneActivation = true;
            
                while (!asyncLoadPre.isDone)
                {
                    yield return null;
                }
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)_eScene);
            asyncLoad.allowSceneActivation = true;
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(_delayBetweenTransitions);
            
            _mainCamera = Camera.main;
            
            // Step 3: Move transition image down to reveal the new scene
            Tween transitionOut = _transitionImageRectTransform.DOAnchorPosY(-WindowHeight, _transitionDuration)
                .SetEase(_outEase);
            
            if (_audioSource != null && _transitionPart2SoundEffect != null)
            {
                _audioSource.clip = _transitionPart2SoundEffect;
                _audioSource.Play();
            }
            
            yield return transitionOut.WaitForCompletion();
            
            if (_useHeavyImpact && _mainCamera != null)
            {
                _mainCamera.DOShakePosition(_impactPauseTime, _cameraShakeStrength/2, _cameraShakeVibrato);
            }
            
            HideTransitionImage();
            
            _isTransitioning = false;
        }
        
    }

}
