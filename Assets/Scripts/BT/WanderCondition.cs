using App;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class WanderCondition : Conditional
    {
        private PlayerController _owner;

        public override void OnAwake()
        {
            _owner = Owner.gameObject.GetComponent<PlayerController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_owner.distEnemy.Value == 3 && _owner.heathLevel.Value > 1)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}
