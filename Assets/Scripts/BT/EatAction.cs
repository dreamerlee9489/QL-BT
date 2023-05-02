using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EatAction : Action
    {
        private float waitDuration;
        private float startTime;
        private float pauseTime;
        private SharedFloat _eatCD;
        private RabbitController _owner;

        public SharedFloat waitTime;

        public override void OnAwake()
        {
            _owner = Owner.gameObject.GetComponent<RabbitController>();
        }

        public override void OnStart()
        {
            startTime = Time.time;
            waitDuration = waitTime.Value;
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            Owner.GetComponent<RabbitController>().goalText.text = "Eat";
        }

        public override TaskStatus OnUpdate()
        {
            if (startTime + waitDuration < Time.time)
            {
                _eatCD.Value = 4;
                _owner.GetDemage(-30);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().goalText.text = "";
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                pauseTime = Time.time;
            }
            else
            {
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnReset()
        {
            waitTime = 1;
        }
    }
}