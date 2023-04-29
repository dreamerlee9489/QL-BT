using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestCondition2 : Conditional
    {
        public SharedFloat utility1 = 0, utility2 = 0;
        public SharedInt hp;

        public override TaskStatus OnUpdate()
        {
            if (utility2.Value < utility1.Value)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override float GetUtility()
        {
            if (hp.Value < 50)
                return utility2.Value = 50;
            return utility2.Value = 0;
        }
    }
}
