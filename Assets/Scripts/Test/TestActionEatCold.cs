using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TestActionEatCold : TestAction
	{
        public override void OnEnd()
        {
            base.OnEnd();
            _hp = (_prevState & 0b100) >> 2;
            _hp = _status == TaskStatus.Success ? Mathf.Clamp(_hp + 1, 0, 1) : _hp;
            _tem = Random.Range(0, 2);
            _cnt = Random.Range(0, 2);
            currentState.Value = (_hp << 2) | (_tem << 1) | _cnt;
        }

        public override float GenReward()
        {
            _hp = (_prevState & 0b100) >> 2;
            _tem = (_prevState & 0b010) >> 1;
            _cnt = (_prevState & 0b001);
            if (_hp != 0)
                return -1;
            else
                return _tem == 0 ? 0 : 10;
        }
    }
}
