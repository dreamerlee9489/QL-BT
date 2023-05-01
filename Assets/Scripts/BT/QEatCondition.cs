using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QEatCondition : Conditional
    {
        private SharedFloat _eatCD;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox, _state;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _state = Owner.GetVariable("State") as SharedInt;
            List<List<float>> qTable = GameMgr.Instance.qTable;
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
            {
                _eatCD.Value = 4;
                return TaskStatus.Success;
            }
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
