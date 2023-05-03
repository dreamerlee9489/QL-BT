using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TestEatCondition : Conditional
    {
        protected SharedInt hp;

        public override void OnAwake()
        {
            hp = Owner.GetVariable("Health") as SharedInt;
        }

        public override TaskStatus OnUpdate()
        {
            if (hp.Value > 50)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
