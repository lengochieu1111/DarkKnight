using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.StateMachine
{
    public abstract class BaseState<Owner> : IState
    {
        protected readonly Owner owner;
        protected readonly Animator animator;

        #region CONTRUCTOR

        protected BaseState(Owner owner, Animator animator)
        {
            this.owner = owner;
            this.animator = animator;
        }

        #endregion

        #region MAIN

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        #endregion

    }

}

