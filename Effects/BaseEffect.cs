using System;
using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseEffect : PoolPrefab
{
    [SerializeField, BoxGroup("ANIMATION")] private bool _deactiveAfterAnimationEnd;
    [SerializeField,BoxGroup("ANIMATION"), Required("ANIMATION"), ShowIf("_deactiveAfterAnimationEnd")] private Animator _animator;
    
    [SerializeField, BoxGroup("PARTICLE SYSTEM")] private bool _deactiveAfterParticleSystemEnd;
    [SerializeField, BoxGroup("PARTICLE SYSTEM"), Required("ANIMATION"), ShowIf("_deactiveAfterParticleSystemEnd")] private ParticleSystem _particleSystem;
    private Coroutine _deactivateCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();

        //## Deactive Affter Animation End
        if (_deactiveAfterAnimationEnd && _animator != null)
        {
            _deactivateCoroutine = StartCoroutine(Deactivate_Coroutine());
        }
        else if (_deactiveAfterParticleSystemEnd && _particleSystem != null)
        {
            // _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        
    }

    private IEnumerator Deactivate_Coroutine()
    {
        float deactivateTime = _animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(deactivateTime);

        Deactivate();

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        /*if (!_deactiveAfterAnimationEnd)
        {
            Deactivate();
        }*/
        
    }

    /*private void OnParticleSystemStopped()
    {
        Debug.Log("OnParticleSystemStopped");
    }*/
}
