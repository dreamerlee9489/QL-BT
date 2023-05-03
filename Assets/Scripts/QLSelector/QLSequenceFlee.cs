namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class QLSequenceFlee : QLSequence
    {
        public override float GetReward(int state)
        {
            if (_hp.Value == 0)
                return -300;
            else
            {
                if (_hp.Value == 1 && _ds.Value > 1 && _de.Value < 2)
                    return 20;
                else if (_ds.Value < 2 || _de.Value > 1)
                    return -1;
                return 0;
            }
        }
    }
}
