using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionAssist : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_de.Value < 2 && _hp.Value > 1 && _nn.Value == 2)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
