using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionFlock : TestAction
    {
        public override void OnEnd()
        {
            base.OnEnd();
            _de = Mathf.Clamp(_de + Random.Range(-1, 2), 0, 3);
            _ds = Mathf.Clamp(_ds + Random.Range(-1, 2), 0, 3);
            _df = Mathf.Clamp(_df + Random.Range(-1, 2), 0, 3);
            _nn = Mathf.Clamp(_nn + Random.Range(-1, 2), 0, 3);
            _hp = _de == 0 ? Mathf.Max(_hp - 1, 0) : _hp;
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
            if (_nn < 2 && _de < 2)
                return -1;
            else
                return 0;
        }

        public override float GetReward(int state)
        {
            throw new System.NotImplementedException();
        }
    }
}
