namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestSimpleAction : Action, IRewarder
    {
        protected int _hp, _tem, _cnt, _area, _prevState;
        protected TaskStatus _status;
        protected SharedInt _currState;

        public abstract float GenReward();
        public abstract double GetReward(int state);

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("State") as SharedInt;
        }

        public override void OnStart()
        {
            _prevState = _currState.Value;
            _hp = (_prevState & 0b1000) >> 3;
            _tem = (_prevState & 0b0100) >> 2;
            _cnt = (_prevState & 0b0010) >> 1;
            _area = (_prevState & 0b0001);
        }
    }
}
