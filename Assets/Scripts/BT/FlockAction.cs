using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FlockAction : NavMeshGroupMovement, IRewarder
    {
        public SharedFloat neighborDistance = 30;
        public SharedFloat lookAheadDistance = 20;
        public SharedFloat alignmentWeight = 0.4f;
        public SharedFloat cohesionWeight = 0.5f;
        public SharedFloat separationWeight = 0.6f;

        private float waitDuration = 2f;
        private float startTime;

        public double GetReward(int state) => 0;

        public override void OnAwake()
        {
            var temp = GameObject.FindGameObjectsWithTag("Rabbit");
            agents = new SharedGameObject[temp.Length];
            for (int i = 0; i < agents.Length; i++)
                agents[i] = new SharedGameObject() { Value = temp[i] };
        }

        public override void OnStart()
        {
            base.OnStart();
            startTime = Time.time;
            //Owner.GetComponent<RabbitController>().goalText.text = "Flock";
        }

        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < agents.Length; ++i)
            {
                if (agents[i].Value.activeSelf)
                {
                    Vector3 alignment, cohesion, separation;
                    DetermineFlockParameters(i, out alignment, out cohesion, out separation);
                    var velocity = alignment * alignmentWeight.Value + cohesion * cohesionWeight.Value + separation * separationWeight.Value;
                    if (velocity.magnitude == 0 && startTime + waitDuration < Time.time)
                        return TaskStatus.Failure;
                    if (!SetDestination(i, transforms[i].position + velocity * lookAheadDistance.Value))
                    {
                        velocity *= -1;
                        SetDestination(i, transforms[i].position + velocity * lookAheadDistance.Value);
                    }
                }
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            //Owner.GetComponent<RabbitController>().goalText.text = "";
        }

        private void DetermineFlockParameters(int index, out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
        {
            alignment = cohesion = separation = Vector3.zero;
            int neighborCount = 0;
            var agentPosition = transforms[index].position;
            for (int i = 0; i < agents.Length; ++i)
            {
                if (index != i)
                {
                    var position = transforms[i].position;
                    if (Vector3.Magnitude(position - agentPosition) < neighborDistance.Value)
                    {
                        alignment += Velocity(i);
                        cohesion += position;
                        separation += position - agentPosition;
                        neighborCount++;
                    }
                }
            }

            if (neighborCount == 0)
                return;
            alignment = (alignment / neighborCount).normalized;
            cohesion = ((cohesion / neighborCount) - agentPosition).normalized;
            separation = ((separation / neighborCount) * -1).normalized;
        }

        public override void OnReset()
        {
            base.OnReset();
            neighborDistance = 100;
            lookAheadDistance = 5;
            alignmentWeight = 0.4f;
            cohesionWeight = 0.5f;
            separationWeight = 0.6f;
        }        
    }
}