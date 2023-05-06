using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
    public class PursueAction : NavMeshMovement, IRewarder
    {
        private Vector3 targetPosition;
        private int _hp, _nn, _df, _ds, _de, _prevState;

        public SharedFloat targetDistPrediction = 20;
        public SharedFloat targetDistPredictionMult = 20;
        public SharedGameObject target;        

        public override void OnStart()
        {
            base.OnStart();
            if (target.Value != null)
            {
                targetPosition = target.Value.transform.position;
                SetDestination(Target());
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
                return TaskStatus.Failure;
            if (HasArrived())
            {
                return TaskStatus.Success;
            }
            SetDestination(Target());
            return TaskStatus.Running;
        }

        private Vector3 Target()
        {
            var distance = (target.Value.transform.position - transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            if (speed <= distance / targetDistPrediction.Value)
                futurePrediction = targetDistPrediction.Value;
            else
                futurePrediction = (distance / speed) * targetDistPredictionMult.Value; // the prediction should be accurate enough

            var prevTargetPosition = targetPosition;
            targetPosition = target.Value.transform.position;
            return targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;
        }

        public override void OnReset()
        {
            base.OnReset();

            targetDistPrediction = 20;
            targetDistPredictionMult = 20;
            target = null;
        }

        public double GetReward(int state)
        {
            _hp = (state & 0b1100000000) >> 8;
            _nn = (state & 0b0011000000) >> 6;
            _df = (state & 0b0000110000) >> 4;
            _ds = (state & 0b0000001100) >> 2;
            _de = (state & 0b0000000011);
            return 0;
        }
    }
}