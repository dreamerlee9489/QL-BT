namespace BehaviorDesigner.Runtime.Tasks
{
    public class EatCondition : Conditional
    {
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedFloat _eatCD;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
        }


        public override TaskStatus OnUpdate()
        {
            if (_eatCD.Value == 0 && _heathLv.Value < 2 && _distFood.Value == 0)
            {
                _eatCD.Value = 4;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
