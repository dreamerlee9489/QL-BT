using App;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QEatCondition : Conditional
    {
        private SharedInt _state, _distFood;
        private SharedFloat _eatCD;
        private readonly Dictionary<int, float> _states = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            float[][] qTable = GameMgr.Instance.Q;
            for (int i = 0; i < 1024; ++i)
                _states.Add(i, qTable[i][(int)ActionSpace.Eat]);
        }

        public override TaskStatus OnUpdate()
        {
            if (_eatCD.Value == 0 && _distFood.Value == 0)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            return _states[_state.Value];
        }
    }
}
