using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestActionPlayGame : TestSimpleAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_hp != 1)
                return TaskStatus.Failure;
            return _status = TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _hp = (_prevState & 0b1000) >> 3;
            _hp = _status == TaskStatus.Success ? Mathf.Clamp(_hp - 1, 0, 1) : _hp;
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
                return _cnt == 0 ? 10 : 0;
        }

        public override double GetReward()
        {
            if (_hp == 0)
                return -1;
            else
                return _cnt == 0 ? 10 : 0;
        }
    }
}
