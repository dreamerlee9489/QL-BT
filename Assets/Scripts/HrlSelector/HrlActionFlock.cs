using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class HrlFlockAction : NavMeshMovement, IRewarder
    {
        private float _pauseTime;
        private float _destinationReachTime;
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;

        public SharedFloat minWanderDistance = 20;
        public SharedFloat maxWanderDistance = 20;
        public SharedFloat wanderRate = 2;
        public SharedFloat minPauseDuration = 0;
        public SharedFloat maxPauseDuration = 0;
        public SharedInt targetRetries = 1;

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
            Owner.GetComponent<RabbitController>().GoalText.text = "Flock";
            if (_distFox.Value < 2 || _neighNum.Value < 2)
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_distFox.Value < 2 || _neighNum.Value < 2)
                return TaskStatus.Failure;
            if (HasArrived())
            {
                if (maxPauseDuration.Value <= 0)
                    TrySetTarget();
                else
                {
                    if (_destinationReachTime == -1)
                    {
                        _destinationReachTime = Time.time;
                        _pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                    }
                    if (_destinationReachTime + _pauseTime <= Time.time)
                    {
                        if (TrySetTarget())
                            _destinationReachTime = -1;
                    }
                }
            }
            else if (navMeshAgent.velocity.sqrMagnitude == 0)
                TrySetTarget();
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().GoalText.text = "";
        }

        private bool TrySetTarget()
        {
            var direction = transform.forward;
            var validDestination = false;
            var attempts = targetRetries.Value;
            var destination = transform.position;
            while (!validDestination && attempts > 0)
            {
                direction = direction + Random.insideUnitSphere * wanderRate.Value;
                destination = transform.position + direction.normalized * Random.Range(minWanderDistance.Value, maxWanderDistance.Value);
                validDestination = SamplePosition(destination);
                attempts--;
            }
            if (validDestination)
            {
                SetDestination(destination);
            }
            return validDestination;
        }

        public override void OnReset()
        {
            minWanderDistance = 20;
            maxWanderDistance = 20;
            wanderRate = 2;
            minPauseDuration = 0;
            maxPauseDuration = 0;
            targetRetries = 1;
        }
    }
}