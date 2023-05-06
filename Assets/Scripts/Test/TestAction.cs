namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestAction : Action, IRewarder
    {
        protected int _hp, _nn, _df, _ds, _de, _prevState;
        protected TaskStatus _status;
        protected SharedInt _currState;

        public abstract double GetReward(int state);
        public virtual float GenReward() => 0;

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("State") as SharedInt;
        }

        public override void OnStart()
        {
            _prevState = _currState.Value;
            _hp = (_prevState & 0b1100000000) >> 8;
            _nn = (_prevState & 0b0011000000) >> 6;
            _df = (_prevState & 0b0000110000) >> 4;
            _ds = (_prevState & 0b0000001100) >> 2;
            _de = (_prevState & 0b0000000011);
        }
    }
}
