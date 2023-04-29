using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class TestAction2 : Action
    {
        public SharedFloat utility2 = 0;

        public override void OnStart()
        {
            base.OnStart();
            //Debug.Log("ACTION 2 START");
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log("PLAY RUN " + GetUtility());
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            //Debug.Log("ACTION 2 END");
        }
    }
}
