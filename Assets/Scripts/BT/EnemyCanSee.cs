using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EnemyCanSee : Conditional
    {
        private SharedFloat _viewDist;
        private SharedGameObject _target;

        public override void OnAwake()
        {
            _viewDist = Owner.GetVariable("ViewDist") as SharedFloat;
            _target = Owner.GetVariable("NearRabbit") as SharedGameObject;
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && _target.Value.GetComponent<RabbitController>().CanBeSee())
            {
                if (Vector3.Distance(transform.position, _target.Value.transform.position) <= _viewDist.Value)
                    return TaskStatus.Success;
                else
                {
                    _target.Value = null;
                    return TaskStatus.Failure;
                }
            }
            for (int i = 0; i < GameMgr.Instance.Rabbits.Count; ++i)
            {
                if (CanSee(GameMgr.Instance.Rabbits[i]))
                {
                    _target.Value = GameMgr.Instance.Rabbits[i];
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

        private bool CanSee(GameObject gameObject)
        {
            if (!gameObject.GetComponent<RabbitController>().CanBeSee()) return false;
            Vector3 direction = gameObject.transform.position - transform.position;
            return direction.magnitude <= _viewDist.Value;
        }
    }
}