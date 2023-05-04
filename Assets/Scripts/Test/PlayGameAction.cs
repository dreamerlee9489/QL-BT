using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class PlayGameAction : TestAction
    {
        public override void OnEnd()
        {
            _hp = (previouState.Value & 0b100) >> 2;
            _hp = _status == TaskStatus.Success ? Mathf.Clamp(_hp - 1, 0, 1) : _hp;
            _tem = Random.Range(0, 2);
            _cnt = Random.Range(0, 2);
            currentState.Value = (_hp << 2) | (_tem << 1) | _cnt;
        }

        public override float GetReward(int state)
        {
            _hp = (state & 0b100) >> 2;
            _tem = (state & 0b010) >> 1;
            _cnt = (state & 0b001);
            if (_hp != 1)
                return -1;
            else
                return _cnt == 0 ? 10 : 0;
        }
    }
}
