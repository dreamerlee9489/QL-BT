using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionSeekFood : TestAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_hp == 3 || _df == 0)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _de = Mathf.Clamp(_de + Random.Range(-1, 2), 0, 3);
            _ds = Mathf.Clamp(_ds + Random.Range(-1, 2), 0, 3);
            _df = Mathf.Max(_df - 1, 0);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _hp = _de == 0 ? Mathf.Max(_hp - 1, 0) : _hp;
            _currState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }

        public override double GetReward()
        {
            if (_hp == 0) return -300;
            if (_hp == 3 || _df == 0) return -1;
            if (_hp == 1 && _df != 0) return 100;
            return 0;
        }
    }
}
