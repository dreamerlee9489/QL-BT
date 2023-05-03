using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionWander : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_de.Value < 2 || _nn.Value > 1)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
