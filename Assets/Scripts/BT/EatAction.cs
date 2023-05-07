using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EatAction : Action, IRewarder
    {
        private double _reward;
        private float _waitDuration, _startTime, _pauseTime;
        private SharedFloat _eatCD;

        public SharedFloat waitTime;

        public double GetReward(int state) => _reward;

        public override void OnStart()
        {
            _reward = 0;
            _startTime = Time.time;
            _waitDuration = waitTime.Value;
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            //Owner.GetComponent<RabbitController>().GoalText.text = "Eat";
        }

        public override TaskStatus OnUpdate()
        {
            if (_startTime + _waitDuration < Time.time)
            {
                _reward = 200;
                _eatCD.Value = 4;
                Owner.GetComponent<RabbitController>().GetDemage(-30);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            //Owner.GetComponent<RabbitController>().GoalText.text = "";
        }

        public override void OnPause(bool paused)
        {
            if (paused)
                _pauseTime = Time.time;
            else
                _startTime += (Time.time - _pauseTime);
        }

        public override void OnReset()
        {
            waitTime = 1;
        }        
    }
}