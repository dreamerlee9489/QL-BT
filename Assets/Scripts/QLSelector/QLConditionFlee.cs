using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionFlee : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_hp.Value < 2 && _de.Value < 2 && _ds.Value > 1)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
