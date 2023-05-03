using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLSelectorIdle : QLSelector
    {
        public override float GetReward(int state)
        {
            return (children[currentChildIndex] as QLSelector).GetReward(state);
        }
    }
}
