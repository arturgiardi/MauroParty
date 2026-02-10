using System;
using System.Collections;
using System.Collections.Generic;

namespace TutzEngine.StateMachine
{
    public class BaseStateMachineManager<T> where T : BaseState
    {
        private Stack<T> Stack { get; }

        public T CurrentState => Stack.Count > 0 ? Stack.Peek() : null;

        public BaseStateMachineManager()
        {
            Stack = new Stack<T>();
        }

        public void PushState(T newState)
        {
            CurrentState?.Exit();
            Stack.Push(newState);
            newState.OnPush();
            newState.Enter();
        }

        public T PopState()
        {
            T state = ExitState();
            CurrentState?.Enter();
            return state;
        }

        private T ExitState()
        {
            var state = Stack.Pop();
            state.Exit();
            return state;
        }

        public void PopTo<U>() where U : T
        {
            ExitState();
            for (int i = Stack.Count; i >= 0; i--)
            {
                var state = Stack.Pop();

                if (state is U)
                {
                    PushState(state);
                    return;
                }
            }
            throw new InvalidOperationException("The required state doesn't exist in the stack");
        }

        public bool Contains<U>() where U : T
        {
            foreach (var item in Stack)
            {
                if (item is U)
                    return true;
            }
            return false;
        }

        public void ClearStack(T newState)
        {
            ExitState();
            Stack.Clear();
            PushState(newState);
        }

        public void SwapState(T newState)
        {
            if (Stack.Count > 0)
                ExitState();
            PushState(newState);
        }
    }

    public class BaseState
    {
        public virtual void OnPush() { }
        public virtual void Exit() { }
        public virtual void Enter() { }
    }
}