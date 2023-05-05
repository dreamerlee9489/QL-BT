using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionEat : TestAction
    {
        public override void OnEnd()
        {
            base.OnEnd();
            _de = Mathf.Clamp(_de + Random.Range(-1, 2), 0, 3);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _hp = _de == 0 ? _hp : Mathf.Min(_hp + 1, 3);
            _currState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }

        public override float GenReward()
        {
            _hp = (_prevState & 0b1100000000) >> 8;
            _nn = (_prevState & 0b0011000000) >> 6;
            _df = (_prevState & 0b0000110000) >> 4;
            _ds = (_prevState & 0b0000001100) >> 2;
            _de = (_prevState & 0b0000000011);
            if (_hp == 0)
                return -300;
            if (_hp == 1 && _df == 0)
                return 200;
            else if (_hp == 3 || _df != 0)
                return -10;
            else
                return 0;
        }

        public override float GetReward(int state)
        {
            throw new System.NotImplementedException();
        }
    }
}
