using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EnemyCanSee : Conditional
    {
        //public SharedFloat viewAngle = 120;
        public SharedFloat viewDist = 30;
        public SharedGameObject target;

        private GameObject[] players;

        public override void OnAwake()
        {
            players = EnemyController.players;
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value != null && target.Value.activeSelf)
                return TaskStatus.Success;
            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].activeSelf && CanSee(players[i]))
                {
                    target.Value = players[i];
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }

        private bool CanSee(GameObject gameObject, SharedFloat viewAngle = null)
        {
            Vector3 direction = gameObject.transform.position - transform.position;
            //return direction.magnitude <= viewDist.Value && Vector3.Angle(direction, transform.forward) < viewAngle.Value;
            return direction.magnitude <= viewDist.Value;
        }
    }
}
