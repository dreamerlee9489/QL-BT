namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class QLSequenceFlock : QLSequence
    {
        public override float GetReward(int state)
        {
            if (_hp.Value == 0)
                return -300;
            else
            {
                if (_nn.Value < 2 || _de.Value < 2)
                    return -1;
                return 0;
            }
        }
    }
}