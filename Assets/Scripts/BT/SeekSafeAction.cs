using App;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SeekSafeAction : NavMeshMovement
    {
        public SharedGameObject target;
        public SharedVector3 targetPosition;

        public override void OnStart()
        {
            base.OnStart();
            SetDestination(Target());
            Owner.GetComponent<RabbitController>().goalText.text = "SeekSafe";
        }

        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                GameMgr.Instance.RabbitEnterSafe(navMeshAgent);
                return TaskStatus.Success;
            }
            SetDestination(Target());
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().goalText.text = "";
        }

        private Vector3 Target()
        {
            if (target.Value != null)
                return target.Value.transform.position;
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}
