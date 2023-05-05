using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TestActionEatCold : TestSimpleAction
	{
        public override TaskStatus OnUpdate()
        {
            if (_hp != 0)
                return TaskStatus.Failure;
            return _status = TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _hp = (_prevState & 0b1000) >> 3;
            _hp = _status == TaskStatus.Success ? Mathf.Clamp(_hp + 1, 0, 1) : _hp;
            _tem = Random.Range(0, 2);
            _cnt = Random.Range(0, 2);
            _area = Random.Range(0, 2);
            _currState.Value = (_hp << 3) | (_tem << 2) | (_cnt << 1) | _area;
        }

        public override float GenReward()
        {
            if (_status == TaskStatus.Failure)
                return -1;
            else
                return _tem == 0 ? 0 : 10;
        }

        public override double GetReward(int state)
        {
            _hp = (state & 0b1000) >> 3;
            _tem = (state & 0b0100) >> 2;
            _cnt = (state & 0b0010) >> 1;
            _area = (state & 0b0001);
            if (_hp == 1)
                return -1;
            else
                return _tem == 0 ? 0 : 10;
        }
    }
}
