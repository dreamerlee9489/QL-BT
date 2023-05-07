using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class HrlFleeAction : NavMeshMovement, IRewarder
    {
        private bool _hasMoved;
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedFloat _fleeCD;
        private GameObject _target;

        public SharedFloat fleedDistance;
        public SharedFloat lookAheadDistance;
        public SharedGameObject target;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            base.OnAwake();
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
        }

        public override void OnStart()
        {
            base.OnStart();
            _reward = 0;
            _hasMoved = false;
            _target = target.Value;
            SetDestination(Target());
            Owner.GetComponent<RabbitController>().GoalText.text = "Flee";
            if (_fleeCD.Value > 0 || _distFox.Value > 1 || _heathLv.Value > 1 || _distSafe.Value < 2)
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_fleeCD.Value > 0 || _distFox.Value > 1 || _heathLv.Value > 1 || _distSafe.Value < 2)
                return TaskStatus.Failure;
            if (Vector3.Magnitude(transform.position - _target.transform.position) > fleedDistance.Value)
            {
                _reward = 20;
                return TaskStatus.Success;
            }
            if (HasArrived())
            {
                if (!_hasMoved)
                {
                    _reward = -5;
                    return TaskStatus.Failure;
                }
                if (!SetDestination(Target()))
                {
                    _reward = -5;
                    return TaskStatus.Failure;
                }
                _hasMoved = false;
            }
            else
            {
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (_hasMoved && velocityMagnitude <= 0f)
                {
                    _reward = -5;
                    return TaskStatus.Failure;
                }
                _hasMoved = velocityMagnitude > 0f;
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            _fleeCD.Value = 4;
            Owner.GetComponent<RabbitController>().GoalText.text = "";
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
