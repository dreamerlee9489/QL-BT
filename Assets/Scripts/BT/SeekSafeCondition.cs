using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SeekSafeCondition : Conditional
    {
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedGameObject _nearSafe;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _nearSafe = Owner.GetVariable("NearSafe") as SharedGameObject;
        }

        public override TaskStatus OnUpdate()
        {
            if (_nearSafe.Value != null && _distFox.Value < 2 && _distSafe.Value < 2 && _heathLv.Value < 2)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
