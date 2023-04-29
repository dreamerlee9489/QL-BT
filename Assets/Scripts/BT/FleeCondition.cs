namespace BehaviorDesigner.Runtime.Tasks
{
    public class FleeCondition : Conditional
	{
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedFloat _fleeCD;

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
            if (_fleeCD.Value == 0 && _heathLv.Value < 2 && _distFox.Value < 2 && _distSafe.Value > 1)
            {
                _fleeCD.Value = 4;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
