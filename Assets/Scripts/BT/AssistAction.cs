using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class AssistAction : Action, IRewarder
    {
        private double _reward;
        private SharedGameObject _target;
        private SharedFloat _arriveDist, _attackCD;
        private SharedInt _demage;

        public double GetReward(int state) => _reward;

        public override void OnAwake()
        {
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            _demage = Owner.GetVariable("Demage") as SharedInt;
            _arriveDist = Owner.GetVariable("ArriveDist") as SharedFloat;
            _attackCD = Owner.GetVariable("AttackCD") as SharedFloat;
        }

        public override void OnStart()
        {
            _reward = 0;
            //Owner.GetComponent<RabbitController>().goalText.text = "Assist";
        }

        public override TaskStatus OnUpdate()
        {
            if (_attackCD.Value > 0)
            {
                _attackCD.Value = Mathf.Max(_attackCD.Value - Time.deltaTime, 0);
                return TaskStatus.Running;
            }
            if (_target.Value != null && _target.Value.activeSelf)
            {
                if (Vector3.Distance(_target.Value.transform.position, Owner.transform.position) <= _arriveDist.Value)
                {
                    _reward += 3;
                    _attackCD = 1;
                    _target.Value.GetComponent<FoxController>().GetDemage(_demage.Value, Owner.GetComponent<RabbitController>());
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            //Owner.GetComponent<RabbitController>().goalText.text = "";
        }
    }
}