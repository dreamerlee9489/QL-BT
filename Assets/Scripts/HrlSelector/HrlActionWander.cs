using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class HrlWanderAction : NavMeshMovement, IRewarder
    {
        private float pauseTime;
        private float destinationReachTime;

        public SharedFloat minWanderDistance = 20;
        public SharedFloat maxWanderDistance = 20;
        public SharedFloat wanderRate = 2;
        public SharedFloat minPauseDuration = 0;
        public SharedFloat maxPauseDuration = 0;
        public SharedInt targetRetries = 1;

        public double GetReward(int state) => 0;

        public override void OnStart()
        {
            base.OnStart();
            Owner.GetComponent<RabbitController>().GoalText.text = "Wander";
        }

        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                if (maxPauseDuration.Value > 0)
                {
                    if (destinationReachTime == -1)
                    {
                        destinationReachTime = Time.time;
                        pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                    }
                    if (destinationReachTime + pauseTime <= Time.time)
                    {
                        if (TrySetTarget())
                            destinationReachTime = -1;
                    }
                }
                else
                {
                    TrySetTarget();
                }
            }
            else if (navMeshAgent.velocity.sqrMagnitude == 0)
            {
                TrySetTarget();
            }
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