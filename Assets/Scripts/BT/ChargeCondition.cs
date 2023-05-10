using App;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeCondition : Conditional, IRewarder
    {
        private bool _isHrl;
        private double _reward;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox;
        private SharedGameObject _target;

        public double GetReward() => _reward;

        public override void OnAwake()
        {
            _heathLv = Owner.GetVariable("HealthLevel") as SharedInt;
            _neighNum = Owner.GetVariable("NeighNum") as SharedInt;
            _distFood = Owner.GetVariable("DistFood") as SharedInt;
            _distSafe = Owner.GetVariable("DistSafe") as SharedInt;
            _distFox = Owner.GetVariable("DistFox") as SharedInt;
            _target = Owner.GetVariable("NearFox") as SharedGameObject;
            _isHrl = GameMgr.Instance.btType == BtType.HRL;
        }

        public override void OnStart()
        {
            if (_isHrl && (_distFox.Value > 1 || _heathLv.Value < 2))
                _reward = -1;
        }

        public override TaskStatus OnUpdate()
        {
            if (_isHrl && (_distFox.Value > 1 || _heathLv.Value < 2))
                return TaskStatus.Failure;
            if (!_isHrl && (_distFox.Value > 1 || _neighNum.Value < 2 || _heathLv.Value != 3))
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }
    }
}
