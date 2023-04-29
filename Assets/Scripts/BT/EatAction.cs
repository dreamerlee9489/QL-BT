using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EatAction : Action
    {
        public SharedFloat waitTime = 1;

        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;
        private RabbitController _owner;

        public override void OnAwake()
        {
            _owner = Owner.gameObject.GetComponent<RabbitController>();
        }

        public override void OnStart()
        {
            // Remember the start time.
            startTime = Time.time;
            waitDuration = waitTime.Value;
        }

        public override TaskStatus OnUpdate()
        {
            // The task is done waiting if the time waitDuration has elapsed since the task was started.
            if (startTime + waitDuration < Time.time)
            {
                _owner.GetDemage(-30);
                return TaskStatus.Success;
            }
            // Otherwise we are still waiting.
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