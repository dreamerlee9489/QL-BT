namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class HrlActionSeekFood : HrlAction
    {
        public override double GetReward(int state)
        {
            if (_hp.Value == 0)
                return -300;
            else
            {
                if (_hp.Value == 1 && _df.Value != 0)
                    return 100;
                else if (_hp.Value == 3 || _df.Value == 0)
                    return -1;
                return 0;
            }
        }
    }
}
