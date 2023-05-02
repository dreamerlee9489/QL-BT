using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QWanderCondition : Conditional
    {
        private SharedInt _state;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            List<List<float>> qTable = GameMgr.Instance.qTable;
            for (int i = 0; i < 1024; ++i)
            {
                float max = qTable[i].Max();
                if (max > 0 && max == qTable[i][(int)ActionSpace.Wander])
                    _bestStates.Add(i, qTable[i][(int)ActionSpace.Wander]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_bestStates.ContainsKey(_state.Value))
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
