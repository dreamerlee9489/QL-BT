using App;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QSeekFoodCondition : Conditional
    {
        private SharedInt _state, _heathLv;
        private SharedGameObject _target;
        private readonly Dictionary<int, float> _states = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _target = Owner.GetVariable("NearFood") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
                _states.Add(i, qTable[i][(int)ActionSpace.SeekFood]);
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && _heathLv.Value != 3)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            return _states[_state.Value];
        }
    }
}
