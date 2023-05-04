using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestAction : Action, IRewarder
    {
        protected int _hp, _tem, _cnt;
        protected float _timer = 0, _cd = 1;
        protected TaskStatus _status;
        protected SharedInt previouState, currentState;
        protected SharedInt prevHp, prevTem, prevCnt;

        public abstract float GetReward(int state);

        public override void OnAwake()
        {
            previouState = Owner.GetVariable("PreviouState") as SharedInt;
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
            prevHp = Owner.GetVariable("Hp") as SharedInt;
            prevTem = Owner.GetVariable("Tem") as SharedInt;
            prevCnt = Owner.GetVariable("Cnt") as SharedInt;
        }

        public override void OnStart()
        {
            previouState.Value = currentState.Value;
        }

        public override TaskStatus OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer > _cd)
            {
                _timer = 0;
                return _status = TaskStatus.Success;
            }
            return _status = TaskStatus.Running;
        }
    }
}
