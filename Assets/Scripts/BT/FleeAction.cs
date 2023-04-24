using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FleeAction : NavMeshMovement
    {
        public SharedFloat fleedDistance = 20;
        public SharedFloat lookAheadDistance = 5;
        public SharedGameObject target;

        public override void OnStart()
        {
            base.OnStart();
            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override TaskStatus OnUpdate()
        {
            float distance = Vector3.Magnitude(transform.position - target.Value.transform.position);
            if (distance > fleedDistance.Value)
                return TaskStatus.Success;
            if (HasArrived() && !SetDestination(Target()))
            {
                SetDestination(Target(-1));
            }

            return TaskStatus.Running;
        }

        // Flee in the opposite direction
        private Vector3 Target(int dir = 1)
        {
            if (dir != 1)
            {
                Vector3 randVec = new Vector3(Random.Range(0.0f, 5.0f), 0, Random.Range(0.0f, 5.0f)) + target.Value.transform.position - transform.position;
                return transform.position + lookAheadDistance.Value * randVec.normalized;
            }
            return transform.position + lookAheadDistance.Value * (transform.position - target.Value.transform.position).normalized;
        }

        // Return false if the position isn't valid on the NavMesh.
        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination))
            {
                return false;
            }
            return base.SetDestination(destination);
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
            fleedDistance = 20;
            lookAheadDistance = 5;
            target = null;
        }
    }
}
