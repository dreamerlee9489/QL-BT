using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestAction : Action, IRewarder
    {
        protected int _hp, _tem, _cnt, _area;
        protected int _nn, _df, _ds, _de, _prevState;
        protected float _timer = 0, _cd = 1, _reward = 0;
        protected TaskStatus _status;
        protected SharedInt _currState;

        public abstract float GenReward();
        public abstract float GetReward(int state);

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("CurrentState") as SharedInt;
        }

        public override void OnStart()
        {
            _reward = 0;
            _prevState = _currState.Value;
            _hp = (_prevState & 0b1000) >> 3;
            _tem = (_prevState & 0b0100) >> 2;
            _cnt = (_prevState & 0b0010) >> 1;
            _area = (_prevState & 0b0001);
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
