using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class FleeCondition : Conditional, IRewarder
	{
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedFloat _fleeCD;

        public double GetReward() => _reward;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
        }

        public override TaskStatus OnUpdate()
        {
            if (_fleeCD.Value > 0 || _heathLv.Value > 1 || _distFox.Value > 1 || _distSafe.Value < 2)
            {
                _reward = -1;
                return TaskStatus.Failure;
            }
            _reward = 0;
            return TaskStatus.Success;
        }
    }
}
