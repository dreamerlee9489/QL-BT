using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionSeekSafe : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_de.Value < 2 && _ds.Value < 2 && _hp.Value < 2)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
