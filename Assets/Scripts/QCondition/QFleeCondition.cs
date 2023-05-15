using App;
using System.Collections.Generic;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QFleeCondition : Conditional
    {
        private SharedInt _state;
        private SharedFloat _fleeCD;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _bestStates = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.Q;
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
