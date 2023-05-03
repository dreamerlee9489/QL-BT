namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class TestAction : Action
	{
        protected SharedInt previouState, currentState;
        protected SharedInt hp, tem, cnt;

        public override void OnAwake()
        {
            previouState = Owner.GetVariable("PreviouState") as SharedInt;
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
            hp = Owner.GetVariable("Hp") as SharedInt;
            tem = Owner.GetVariable("Tem") as SharedInt;
            cnt = Owner.GetVariable("Cnt") as SharedInt;
        }

        public override void OnStart()
        {
            previouState.Value = currentState.Value;
            hp.Value = (previouState.Value & 0b110000) >> 4;
            tem.Value = (previouState.Value & 0b001100) >> 2;
            cnt.Value = (previouState.Value & 0b000011);
        }

        public abstract float GetReward();
    }
}
