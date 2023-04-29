using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FleeAction : NavMeshMovement
    {
        public SharedFloat fleedDistance;
        public SharedFloat lookAheadDistance;
        public SharedGameObject target;

        private bool hasMoved;

        public override void OnStart()
        {
            base.OnStart();
            hasMoved = false;
            SetDestination(Target());
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Magnitude(transform.position - target.Value.transform.position) > fleedDistance.Value)
                return TaskStatus.Success;

            if (HasArrived())
            {
                if (!hasMoved)
                    return TaskStatus.Failure;
                if (!SetDestination(Target()))
                    return TaskStatus.Failure;
                hasMoved = false;
            }
            else
            {
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (hasMoved && velocityMagnitude <= 0f)
                    return TaskStatus.Failure;
                hasMoved = velocityMagnitude > 0f;
            }

            return TaskStatus.Running;
        }

        private Vector3 Target()
        {
            return transform.position + (transform.position - target.Value.transform.position).normalized * lookAheadDistance.Value;
        }

        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination))
                return false;
            return base.SetDestination(destination);
        }

        public override void OnReset()
        {
            base.OnReset();
            fleedDistance = 20;
            lookAheadDistance = 5;
            target = null;
        }
    }
}
