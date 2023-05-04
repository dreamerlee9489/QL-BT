using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestAction : Action, IRewarder
    {
        protected int _hp, _tem, _cnt, _nn, _df, _ds, _de, _prevState;
        protected float _timer = 0, _cd = 1, _reward = 0;
        protected TaskStatus _status;
        protected SharedInt currentState;

        public abstract float GenReward();
        public float GetReward() => _reward;

        public override void OnAwake()
        {
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
        }

        public override void OnStart()
        {
            _reward = 0;
            _prevState = currentState.Value;
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

        public override void OnEnd()
        {
            _reward = GenReward();
        }
    }
}
