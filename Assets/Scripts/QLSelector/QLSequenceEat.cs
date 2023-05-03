namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}SequenceIcon.png")]
    public class QLSequenceEat : QLSequence
    {
        public override float GetReward(int state)
        {
            if (_hp.Value == 0)
                return -300;
            else
            {
                if (_hp.Value == 1 && _df.Value == 0)
                    return 200;
                else if (_hp.Value == 3 || _df.Value != 0)
                    return -1;
                return 0;
            }
        }
    }
}
