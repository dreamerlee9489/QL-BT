using App;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class FleeCondition : Conditional
	{
        private PlayerController _owner;

        public override void OnAwake()
        {
            _owner = Owner.gameObject.GetComponent<PlayerController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_owner.distEnemy.Value < 3 && _owner.distSafe.Value > 1 && _owner.neighNum.Value < 2)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
