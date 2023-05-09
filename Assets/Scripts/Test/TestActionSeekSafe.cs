using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionSeekSafe : TestAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_ds == 0 || _de > 1)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _ds = 0;
            _df = Mathf.Clamp(_df + Random.Range(-1, 2), 0, 3);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _currState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }

        public override double GetReward()
        {
            if (_hp == 0) return -300;
            if (_ds == 0 || _de > 1) return -1;
            if (_hp == 1 && _ds == 1 && _de < 2) return 300;
            return 0;
        }
    }
}
