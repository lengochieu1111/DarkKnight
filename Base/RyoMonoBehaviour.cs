using System;
using UnityEngine;

public abstract class RyoMonoBehaviour : MonoBehaviour
{
    #region UNITY CORE

    protected virtual void Awake()
    {
        InitializeValues();
        SetupComponents();
        SetupValues();
    }

    protected virtual void OnEnable()
    {
        ResetComponents();
        ResetValues();
    }

    protected virtual void Reset()
    {
        InitializeValues();
        SetupComponents();
        SetupValues();
        ResetComponents();
        ResetValues();
    }

    protected virtual void Start() { }

    protected virtual void OnDisable() { }
    
    protected virtual void OnDestroy() { }

    protected virtual void OnValidate() { }

    #endregion

    #region SETUP/RESET

    protected virtual void InitializeValues() { }   
    protected virtual void SetupComponents() { }   
    
    protected virtual void SetupValues() { }    
    
    protected virtual void ResetComponents() { }
    
    protected virtual void ResetValues() { }

    #endregion



}
