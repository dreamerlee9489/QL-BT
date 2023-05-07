namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeCondition : Conditional, IRewarder
    {
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedGameObject _target;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
        }

        public override void OnStart()
        {
            if (_distFox.Value > 1 || _heathLv.Value != 3)
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_distFox.Value > 1 || _heathLv.Value != 3)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
