using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class AssistAction : Action
    {
        private SharedGameObject _target;
        private SharedFloat _arriveDist, _attackCD;
        private SharedInt _demage;

        public override void OnAwake()
        {
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            _demage = Owner.GetVariable("Demage") as SharedInt;
            _arriveDist = Owner.GetVariable("ArriveDist") as SharedFloat;
            _attackCD = Owner.GetVariable("AttackCD") as SharedFloat;
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
                    _attackCD = 1;
                    _target.Value.GetComponent<FoxController>().GetDemage(_demage.Value, Owner.gameObject);
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }
    }
}