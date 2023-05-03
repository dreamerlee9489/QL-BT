using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionEat : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_hp.Value < 2 && _df.Value == 0)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
