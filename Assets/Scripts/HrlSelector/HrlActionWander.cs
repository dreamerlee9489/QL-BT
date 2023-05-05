namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class HrlActionWander : HrlAction
    {
        public override double GetReward(int state)
        {
            if (_hp.Value == 0)
                return -300;
            else
            {
                if (_nn.Value > 1 || _de.Value < 2)
                    return -1;
                return 0;
            }
        }
    }
}
