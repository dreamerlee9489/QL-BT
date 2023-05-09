using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class HrlSequence : Sequence, IRewarder
    {
        protected double _totalReward;
        protected SharedInt _currState;

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("State") as SharedInt;
        }

        public override void OnStart()
        {
            _totalReward = 0;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            _totalReward += (children[currentChildIndex++] as IRewarder).GetReward();
            executionStatus = childStatus;
        }

        public double GetReward()
        {
            return _totalReward;
        }
    }
}

