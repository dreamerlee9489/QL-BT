using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class HrlSequence : Sequence, IRewarder
    {
        protected int _prevState;
        protected double _totalReward;
        protected SharedInt _currState;

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("State") as SharedInt;
        }

        public override void OnStart()
        {
            _totalReward = 0;
            _prevState = _currState.Value;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            _totalReward += (children[currentChildIndex++] as IRewarder).GetReward(_prevState);
            executionStatus = childStatus;
        }

        public double GetReward(int state)
        {
            return _totalReward;
        }
    }
}

