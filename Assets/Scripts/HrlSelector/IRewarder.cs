namespace BehaviorDesigner.Runtime.Tasks
{
    public interface IRewarder
    {
        public double GetReward(int state);
    }
}
