using App;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SeekSafeAction : NavMeshMovement, IRewarder
    {
        private double _reward;

        public SharedGameObject target;
        public SharedVector3 targetPosition;

        public double GetReward() => _reward;

        public override void OnStart()
        {
            base.OnStart();
            _reward = 0;
            SetDestination(Target());
            Owner.GetComponent<RabbitController>().GoalText.text = "SeekSafe";
        }

        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                _reward = 300;
                GameMgr.Instance.RabbitEnterSafe(navMeshAgent);
                return TaskStatus.Success;
            }
            SetDestination(Target());
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().GoalText.text = "";
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
