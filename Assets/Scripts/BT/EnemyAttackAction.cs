using App;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EnemyAttackAction : Action
	{
		public SharedGameObject target;
        public SharedInt demage;

        public override TaskStatus OnUpdate()
        {
            if(target.Value != null) 
            {
                target.Value.GetComponent<PlayerController>().GetDemage(demage.Value, Owner.gameObject);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
