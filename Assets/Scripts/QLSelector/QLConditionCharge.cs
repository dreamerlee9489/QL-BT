using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionCharge : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_de.Value < 2 && _hp.Value > 1 && _nn.Value == 3)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}