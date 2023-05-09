using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeAction : Action, IRewarder
    {
        private double _reward;
        private SharedInt _demage, _neighNum;
        private SharedFloat _arriveDist, _attackCD;
        private SharedGameObject _target;

        public double GetReward() => _reward;

        public override void OnAwake()
        {
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _demage = Owner.GetVariable("Demage") as SharedInt;
            _arriveDist = Owner.GetVariable("ArriveDist") as SharedFloat;
            _attackCD = Owner.GetVariable("AttackCD") as SharedFloat;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
        }

        public override void OnStart()
        {
            _reward = 0;
            Owner.GetComponent<RabbitController>().GoalText.text = "Charge";
        }

        public override TaskStatus OnUpdate()
        {
            if (_attackCD.Value > 0)
                return TaskStatus.Running;
            if (_target.Value != null && _target.Value.activeSelf)
            {
                if (Vector3.Distance(_target.Value.transform.position, Owner.transform.position) <= _arriveDist.Value)
                {
                    _reward += 3 * (1 + 0.5 * _neighNum.Value);
                    _attackCD.Value = 1;
                    _target.Value.GetComponent<FoxController>().GetDemage(_demage.Value, Owner.GetComponent<RabbitController>());
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            Owner.GetComponent<RabbitController>().GoalText.text = "";
        }        
    }
}