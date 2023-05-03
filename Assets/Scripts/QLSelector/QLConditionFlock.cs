using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLConditionFlock : QLCondition
    {
        public override TaskStatus OnUpdate()
        {
            if (_de.Value < 2 || _nn.Value < 2)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
