using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QFleeCondition : Conditional
    {
        private SharedFloat _fleeCD;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox, _state;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _state = Owner.GetVariable("State") as SharedInt;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            List<List<float>> qTable = GameMgr.Instance.qTable;
            for (int i = 0; i < 1024; ++i)
            {
                float max = qTable[i].Max();
                if (max > 0 && max == qTable[i][0])
                    _bestStates.Add(i, qTable[i][0]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && _fleeCD.Value == 0 && _bestStates.ContainsKey(_state.Value))
            {
                _fleeCD.Value = 4;
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
