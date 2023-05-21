using App;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QFleeCondition : Conditional
    {
        private SharedInt _state;
        private SharedFloat _fleeCD;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _states = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
                _states.Add(i, qTable[i][(int)ActionSpace.Flee]);
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && _fleeCD.Value == 0)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            return _states[_state.Value];
        }
    }
}
