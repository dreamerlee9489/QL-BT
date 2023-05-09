using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionEat : TestAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_hp == 3 || _df != 0)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _de = Mathf.Clamp(_de + Random.Range(-1, 2), 0, 3);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _hp = _de == 0 ? _hp : Mathf.Min(_hp + 1, 3);
            _currState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }

        public override double GetReward()
        {
            if (_hp == 0) return -300;
            if (_hp == 3 || _df != 0) return -1;
            if (_hp == 1 && _df == 0) return 200;
            return 0;
        }
    }
}
