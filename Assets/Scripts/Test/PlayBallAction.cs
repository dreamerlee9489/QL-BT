using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class PlayBallAction : TestAction
    {
        private float timer = 0, cd = 3;

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer > cd)
            {
                timer = 0;
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        public override float GetReward()
        {
            if (hp.Value != 1)
                return -1;
            else
                return cnt.Value == 0 ? 0 : 10;
        }
    }
}
