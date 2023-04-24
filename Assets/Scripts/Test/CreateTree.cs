using UnityEngine;
using BehaviorDesigner.Runtime;

namespace App
{
	public class CreateTree : MonoBehaviour
	{
		public ExternalBehaviorTree behaviorTree;

        private void Start()
        {
            var bt = gameObject.AddComponent<BehaviorTree>();
            bt.ExternalBehavior = behaviorTree;
            bt.StartWhenEnabled = false;
            //bt.EnableBehavior();
            //bt.DisableBehavior();
        }
    }
}
