using App;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class HrlSeekSafeAction : NavMeshMovement, IRewarder
    {
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;

        public SharedGameObject target;
        public SharedVector3 targetPosition;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            base.OnAwake();
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
        }

        public override void OnStart()
        {
            base.OnStart();
            _reward = 0;
            SetDestination(Target());
            Owner.GetComponent<RabbitController>().GoalText.text = "SeekSafe";
            if (_distFox.Value > 1 || _heathLv.Value > 1 || _distSafe.Value > 1)
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_distFox.Value > 1 || _heathLv.Value > 1 || _distSafe.Value > 1)
                return TaskStatus.Failure;
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
