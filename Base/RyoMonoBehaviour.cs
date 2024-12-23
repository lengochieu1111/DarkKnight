using UnityEngine;

public abstract class RyoMonoBehaviour : MonoBehaviour
{
    #region UNITY CORE

    protected virtual void Awake()
    {
        this.SetupComponents();
        this.SetupValues();
    }

    protected virtual void OnEnable()
    {
        this.ResetComponents();
        this.ResetValues();
    }

    protected virtual void Reset()
    {
        this.SetupComponents();
        this.SetupValues();
        this.ResetComponents();
        this.ResetValues();
    }

    protected virtual void Start() { }

    protected virtual void OnDisable() { }
    
    protected virtual void OnDestroy() { }
    
    #endregion

    #region SETUP/RESET

    protected virtual void SetupComponents() { }   
    
    protected virtual void SetupValues() { }    
    
    protected virtual void ResetComponents() { }
    
    protected virtual void ResetValues() { }

    #endregion



}
