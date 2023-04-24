using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace App
{
	public class MoveTowards : Action
	{
		public float speed = 0;
		public SharedTransform target;

        public override TaskStatus OnUpdate()
        {
            if (target != null && target.Value != null)
            {
                if (Vector3.SqrMagnitude(transform.position - target.Value.position) < 0.1f)
                    return TaskStatus.Success;
                transform.position = Vector3.MoveTowards(transform.position, target.Value.position, speed * Time.deltaTime);
            }
            return TaskStatus.Running;
        }
    }
}
