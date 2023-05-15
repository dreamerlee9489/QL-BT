using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QSeekFoodCondition : Conditional
    {
        private SharedInt _state;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _target = Owner.GetVariable("NearFood") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
            {
                float max = qTable[i].Max();
                if (max > 0 && max == qTable[i][(int)ActionSpace.SeekFood])
                    _bestStates.Add(i, qTable[i][(int)ActionSpace.SeekFood]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && _bestStates.ContainsKey(_state.Value))
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
