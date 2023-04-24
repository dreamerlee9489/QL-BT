using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace App
{
	public class WithinSight : Conditional
	{
		public float fieldOfViewAngle;
		public string targetTag;
		public SharedTransform target;

		private Transform[] possibleTargets;

        public override void OnAwake()
        {
			var targets = GameObject.FindGameObjectsWithTag(targetTag);
			possibleTargets = new Transform[targets.Length];
			for (int i = 0; i < targets.Length; i++)
				possibleTargets[i] = targets[i].transform;
        }

        public override TaskStatus OnUpdate()
        {
			for (int i = 0; i < possibleTargets.Length; ++i)
			{
				if (withinSight(possibleTargets[i], fieldOfViewAngle))
				{
					target.Value = possibleTargets[i];
					return TaskStatus.Success;
				}
			}
			return TaskStatus.Failure;
        }

		public bool withinSight(Transform targetTransform, float fieldOfViewAngle)
		{
			Vector3 direction = targetTransform.position - transform.position;
			return Vector3.Angle(direction, transform.forward) < fieldOfViewAngle;
		}
    }
}
