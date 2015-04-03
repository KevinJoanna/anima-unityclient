﻿using System;
using System.Collections.Generic;

namespace UnityNet.FSM
{
    /************************************************************************/
    /* 有限状态机状态实现类                                                               
    /************************************************************************/
    public class FSMState
    {
        private int stateName;
        private Dictionary<int, int> transitions = new Dictionary<int, int>();

        public void AddTransition(int transition, int outputState)
        {
            this.transitions[transition] = outputState;
        }

        public int ApplyTransition(int transition)
        {
            int stateName = this.stateName;
            if (this.transitions.ContainsKey(transition))
            {
                stateName = this.transitions[transition];
            }
            return stateName;
        }

        public int GetStateName()
        {
            return this.stateName;
        }

        public void SetStateName(int newStateName)
        {
            this.stateName = newStateName;
        }
    }
}

