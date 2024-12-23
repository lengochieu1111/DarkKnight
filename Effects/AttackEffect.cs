using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Interface;
using UnityEngine;

public class AttackEffect : BaseEffect, IAttackable
{
    [SerializeField, Required] private BoxCollider2D _hitCollider;
    [SerializeField] private LayerMask _hitLayer;
    private bool _isTracing;
    private List<HittableObject> _hitedList = new List<HittableObject>();

    protected override void ResetValues()
    {
        base.ResetValues();

        _isTracing = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(_hitCollider.bounds.center, _hitCollider.bounds.size);

    }

    private void Update()
    {
        if (_isTracing)
        {
            ITracing();
        }
        
    }

    #region IATTACKABLE

    public void IBeginAttack() { }

    public void IAttacking() { }

    public void IFinishAttack() { }

    public void IBeginTrace()
    {
        _isTracing = true;
        _hitedList.Clear();
    }

    public void ITracing()
    {
        RaycastHit2D[] hitArray = Physics2D.BoxCastAll(_hitCollider.bounds.center, _hitCollider.bounds.size, 0f, Vector2.right, 0f, _hitLayer);

        foreach (RaycastHit2D hit in hitArray)
        {
            if (hit.transform.TryGetComponent(out HittableObject hittableObject) && !_hitedList.Contains(hittableObject))
            {
                HitData hitData = new HitData();
                hittableObject.IHit(hitData);

                _hitedList.Add(hittableObject);
            }
        }
    }

    public void IFinishTrace()
    {
        _isTracing = false;
    }

    #endregion

    //# ANIMATION EVENT FUNCTION

    private void AE_BeginTrace()
    {
        IBeginTrace();
    }
    
    private void AE_FinishTrace()
    {
        IFinishTrace();
    }


}
