using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FleeAction : NavMeshMovement, IRewarder
    {
        private bool _hasMoved;
        private double _reward;
        private GameObject _target;
        private SharedFloat _fleeCD;

        public SharedFloat fleedDistance;
        public SharedFloat lookAheadDistance;
        public SharedGameObject target;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            base.OnAwake();
            _fleeCD = Owner.GetVariable("FleeCD") as SharedFloat;
        }

        public override void OnStart()
        {
            base.OnStart();
            _reward = 0;
            _hasMoved = false;
            _target = target.Value;
            //Owner.GetComponent<RabbitController>().GoalText.text = "Flee";
            SetDestination(Target());
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Magnitude(transform.position - _target.transform.position) > fleedDistance.Value)
            {
                _reward = 20;
                _fleeCD.Value = 4;
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
            //Owner.GetComponent<RabbitController>().GoalText.text = "";
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
