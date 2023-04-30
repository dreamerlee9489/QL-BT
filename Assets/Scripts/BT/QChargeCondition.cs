using App;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QChargeCondition : Conditional
    {
        private SharedGameObject _target;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox, _state;
        private Dictionary<int, float> bestStates = new();

        public override void OnAwake()
        {
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
                if (max > 0 && max == qTable[i][(int)ActionSpace.Charge])
                    bestStates.Add(i, qTable[i][(int)ActionSpace.Charge]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && bestStates.ContainsKey(_state.Value))
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            if (bestStates.ContainsKey(_state.Value))
                return bestStates[_state.Value];
            return 0;
        }
    }
}
