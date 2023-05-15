using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QEatCondition : Conditional
    {
        private SharedFloat _eatCD;
        private SharedInt _state;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            _state = Owner.GetVariable("State") as SharedInt;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
            {
                float max = qTable[i].Max();
                if (max > 0 && max == qTable[i][(int)ActionSpace.Eat])
                    _bestStates.Add(i, qTable[i][(int)ActionSpace.Eat]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_eatCD.Value == 0 && _bestStates.ContainsKey(_state.Value))
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            if (_bestStates.ContainsKey(_state.Value))
                return _bestStates[_state.Value];
            return 0;
        }
    }
}
