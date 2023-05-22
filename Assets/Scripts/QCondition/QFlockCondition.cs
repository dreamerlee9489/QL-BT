using App;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QFlockCondition : Conditional
    {
        private SharedInt _state;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _states = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
                _states.Add(i, qTable[i][(int)ActionSpace.Flock]);
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override float GetUtility()
        {
            return _states[_state.Value];
        }
    }
}
