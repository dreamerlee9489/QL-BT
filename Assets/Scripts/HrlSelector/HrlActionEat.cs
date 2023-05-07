using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class HrlEatAction : Action, IRewarder
    {
        private double _reward;
        private float _waitDuration, _startTime, _pauseTime;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedFloat _eatCD;

        public SharedFloat waitTime;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            base.OnAwake();
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
        }

        public override void OnStart()
        {
            _reward = 0;
            _startTime = Time.time;
            _waitDuration = waitTime.Value;
            Owner.GetComponent<RabbitController>().GoalText.text = "Eat";
        }

        public override TaskStatus OnUpdate()
        {
            if (_eatCD.Value > 0 || _distFood.Value != 0 || _heathLv.Value > 2 || _distFox.Value < 2)
            {
                _reward = -1;
                return TaskStatus.Failure;
            }
            if (_startTime + _waitDuration < Time.time)
            {
                _reward = 100;
                Owner.GetComponent<RabbitController>().GetDemage(-30);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            _eatCD.Value = 4;
            Owner.GetComponent<RabbitController>().GoalText.text = "";
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