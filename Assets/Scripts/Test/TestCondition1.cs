using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TestCondition1 : Conditional
    {
        public SharedFloat utility1 = 0, utility2 = 0;
        public SharedInt hp;

        public override TaskStatus OnUpdate()
        {
            if (utility1.Value < utility2.Value)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override float GetUtility()
        {
            if (hp.Value < 50)
                return utility1.Value = 0;
            return utility1.Value = 50;
        }
    }
}
