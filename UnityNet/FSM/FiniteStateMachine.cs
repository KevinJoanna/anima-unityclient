using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityNet.FSM
{
    /************************************************************************/
    /* 有限状态机实现                                                         
    /************************************************************************/
    public class FiniteStateMachine
    {
        private int currentStateName;
        private object locker = new object();
        public OnStateChangeDelegate onStateChange;
        private List<FSMState> states = new List<FSMState>();

        public void AddAllStates(Type statesEnumType)
        {
            IEnumerator enumerator = Enum.GetValues(statesEnumType).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this.AddState(current);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public void AddState(object st)
        {
            int newStateName = (int) st;
            FSMState item = new FSMState();
            item.SetStateName(newStateName);
            this.states.Add(item);
        }

        public void AddStateTransition(object from, object to, object tr)
        {
            int st = (int) from;
            int outputState = (int) to;
            int transition = (int) tr;
            this.FindStateObjByName(st).AddTransition(transition, outputState);
        }

        public int ApplyTransition(object tr)
        {
            object locker = this.locker;
            lock (locker)
            {
                int transition = (int) tr;
                int currentStateName = this.currentStateName;
                this.currentStateName = this.FindStateObjByName(this.currentStateName).ApplyTransition(transition);
                if ((currentStateName != this.currentStateName) && (this.onStateChange != null))
                {
                    this.onStateChange(currentStateName, this.currentStateName);
                }
                return this.currentStateName;
            }
        }

        private FSMState FindStateObjByName(object st)
        {
            int num = (int) st;
            foreach (FSMState state in this.states)
            {
                if (num.Equals(state.GetStateName()))
                {
                    return state;
                }
            }
            return null;
        }

        public int GetCurrentState()
        {
            object locker = this.locker;
            lock (locker)
            {
                return this.currentStateName;
            }
        }

        public void SetCurrentState(object state)
        {
            int toStateName = (int) state;
            if (this.onStateChange != null)
            {
                this.onStateChange(this.currentStateName, toStateName);
            }
            this.currentStateName = toStateName;
        }

        public delegate void OnStateChangeDelegate(int fromStateName, int toStateName);
    }
}

