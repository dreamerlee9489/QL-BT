using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeAction : Action
    {
        public SharedFloat waitTime = 1;
        public SharedGameObject target;

        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;

        public override void OnStart()
        {
            // Remember the start time.
            startTime = Time.time;
            waitDuration = waitTime.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (!target.Value.activeSelf)
                return TaskStatus.Failure;
            if (startTime + waitDuration < Time.time)
            {
                target.Value.GetComponent<EnemyController>().GetDemage(5, Owner.gameObject);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                // Remember the time that the behavior was paused.
                pauseTime = Time.time;
            }
            else
            {
                // Add the difference between Time.time and pauseTime to figure out a new start time.
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            waitTime = 1;
        }
    }
}