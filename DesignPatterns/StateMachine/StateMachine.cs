using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.StateMachine
{
    public class StateMachine
    {
        public event Action OnChangeState;
        
        private StateNode _previousNodeState;
        private StateNode _currentNodeState;
        private Dictionary<Type, StateNode> _nodes = new();
        private HashSet<ITransition> _anyTransitions = new();

        #region CORE

            public void Update()
            {
                var transition = GetTransition();
                if (transition != null)
                {
                    ChangeState(transition.To);
                }

                _currentNodeState.State?.Update();
            }

            public void FixedUpdate()
            {
                _currentNodeState.State?.FixedUpdate();
            }

        #endregion

        #region MAIN

            public void SetState(IState state)
            {
                _previousNodeState = _currentNodeState;
                _currentNodeState = _nodes[state.GetType()];
                _currentNodeState.State?.OnEnter();
                
                OnChangeState?.Invoke();
            }
    
            private void ChangeState(IState state)
            {
                if (state == _currentNodeState.State) return;
    
                var previousState = _currentNodeState.State;
                var nextState = _nodes[state.GetType()].State;
    
                previousState?.OnExit();
                nextState?.OnEnter();
    
                _previousNodeState = _currentNodeState;
                _currentNodeState = _nodes[state.GetType()];
                
                OnChangeState?.Invoke();
            }
    
            ITransition GetTransition()
            {
                foreach (var transition in _anyTransitions)
                    if (transition.Condition.Evaluate())
                        return transition;
    
                foreach (var transition in _currentNodeState.Transitions)
                    if (transition.Condition.Evaluate())
                        return transition;
    
                return null;
            }
    
            public void AddTransition(IState from, IState to, IPredicate condition)
            {
                GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
            }
    
            public void AddAnyTransition(IState to, IPredicate condition)
            {
                _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
            }
    
            StateNode GetOrAddNode(IState state)
            {
                var node = _nodes.GetValueOrDefault(state.GetType());
    
                if (node == null)
                {
                    node = new StateNode(state);
                    _nodes.Add(state.GetType(), node);
                }
    
                return node;
            }

        #endregion
        
        class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }

    }

    /*    TRANSITION STATE
     
    #region STATE MACHINE

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        _stateMachine.AddTransition(from, to, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _stateMachine.AddAnyTransition(to, condition);
    }

    #endregion*/

}
