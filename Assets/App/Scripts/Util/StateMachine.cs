using System;
using System.Collections.Generic;

namespace App.Util
{
    public class StateMachine<TContext>
    {
        public abstract class State
        {
            protected TContext Context => StateMachine.Context;

            protected StateMachine<TContext> StateMachine { get; private set; }

            public void SetStateMachine(StateMachine<TContext> sm) => StateMachine = sm;

            public virtual void OnEnter() { }

            public virtual void OnUpdate() { }

            public virtual void OnExit() { }
        }

        public State CurrentState { get; private set; }

        TContext Context { get; }

        readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();

        public StateMachine(TContext context)
        {
            Context = context;
        }
        
        public void AddState<TState>() where TState : State, new()
        {
            var type = typeof(TState);

            if (_states.ContainsKey(type))
            {
                throw new Exception("This state already exists.");
            }

            var state = new TState();
            state.SetStateMachine(this);
            _states.Add(type, state);
        }
        
        public void Transit<TState>() where TState : State
        {
            var type = typeof(TState);
            
            if (!_states.ContainsKey(type))
            {
                throw new Exception("This state does not exist.");
            }
            
            ChangeState(_states[type]);
        }
        
        public void UpdateState()
        {
            CurrentState.OnUpdate();
        }

        void ChangeState(State nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            CurrentState?.OnExit();

            CurrentState = nextState;
            CurrentState.OnEnter();
        }
    }
}
