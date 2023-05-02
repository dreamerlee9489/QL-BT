using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FleeAction : NavMeshMovement
    {
        public SharedFloat fleedDistance;
        public SharedFloat lookAheadDistance;
        public SharedGameObject target;

        private bool hasMoved;
        private SharedFloat _fleeCD;
        private GameObject _target;

        public override void OnStart()
        {
            base.OnStart();
            hasMoved = false;
            _target = target.Value;
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
            SetDestination(Target());
            Owner.GetComponent<RabbitController>().goalText.text = "Flee";
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Magnitude(transform.position - _target.transform.position) > fleedDistance.Value)
            {
                _fleeCD.Value = 4;
                return TaskStatus.Success;
            }

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

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().goalText.text = "";
        }

        private Vector3 Target()
        {
            return transform.position + (transform.position - _target.transform.position).normalized * lookAheadDistance.Value;
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
