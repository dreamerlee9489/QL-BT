using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QLSelectorExplore : QLSelector
    {
        public override float GetReward(int state)
        {
            return (children[currentChildIndex] as QLSequence).GetReward(state);
        }
    }
}
