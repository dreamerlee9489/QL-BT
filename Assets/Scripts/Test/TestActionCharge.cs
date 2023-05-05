using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionCharge : TestAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_nn == 3 && _hp > 1 && _de < 2)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _de = Mathf.Max(_de - 1, 0);
            _ds = Mathf.Clamp(_ds + Random.Range(-1, 2), 0, 3);
            _df = Mathf.Clamp(_df + Random.Range(-1, 2), 0, 3);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _hp = _de == 0 ? Mathf.Max(_hp - 1, 0) : _hp;
            _currState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }

        public override double GetReward(int state)
        {
            _hp = (state & 0b1100000000) >> 8;
            _nn = (state & 0b0011000000) >> 6;
            _df = (state & 0b0000110000) >> 4;
            _ds = (state & 0b0000001100) >> 2;
            _de = (state & 0b0000000011);
            if (_nn == 3 && _hp > 1 && _de < 2) return 12;
            return -1;
        }
    }
}
