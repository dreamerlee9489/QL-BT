namespace BehaviorDesigner.Runtime.Tasks
{
    public class SeekFoodCondition : Conditional
    {
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedGameObject _nearFood;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _nearFood = Owner.GetVariable("NearFood") as SharedGameObject;
        }

        public override TaskStatus OnUpdate()
        {
            if (_nearFood.Value != null && _heathLv.Value < 2 && _distFood.Value > 0)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
