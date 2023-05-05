using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public abstract class HrlAction : Sequence
    {
        protected SharedInt _previouState, _currentState;
        protected SharedInt _hp, _nn, _df, _ds, _de;

        public abstract double GetReward(int state);

        public override void OnAwake()
        {
            _previouState = Owner.GetVariable("PreviouState") as SharedInt;
            _currentState = Owner.GetVariable("CurrentState") as SharedInt;
            _hp = Owner.GetVariable("Health") as SharedInt;
            _nn = Owner.GetVariable("NeighNum") as SharedInt;
            _df = Owner.GetVariable("DistFood") as SharedInt;
            _ds = Owner.GetVariable("DistSafe") as SharedInt;
            _de = Owner.GetVariable("DistFox") as SharedInt;
        }

        public override void OnStart()
        {
            _previouState.Value = _currentState.Value;
            //Debug.Log($"{GetType().Name} start: {_previouState.Value}");
        }
    }
}

