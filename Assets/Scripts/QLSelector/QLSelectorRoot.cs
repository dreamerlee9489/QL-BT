using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLSelectorRoot : QLSelector
    {
        public override float GetReward(int state)
        {
            return (children[currentChildIndex] as QLSelector).GetReward(state);
        }
    }
}
