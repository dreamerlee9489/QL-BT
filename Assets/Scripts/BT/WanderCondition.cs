using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class WanderCondition : Conditional, IRewarder
    {
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
        }

        public override void OnStart()
        {
            Owner.GetComponent<RabbitController>().GoalText.text = "Wander";
            if (_distFox.Value < 2 || _neighNum.Value > 1)
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_distFox.Value < 2 || _neighNum.Value > 1)
                return TaskStatus.Failure;
            if (_distFood.Value < 2 && _heathLv.Value == 1)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
