using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class TestAction1 : Action
	{
        public SharedFloat utility1 = 0;

        public override void OnStart()
        {
            base.OnStart();
            //Debug.Log("ACTION 1 START");
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log("EAT RUN " + GetUtility());
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            //Debug.Log("ACTION 1 END");
        }
    }
}
