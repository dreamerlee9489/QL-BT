using App;
using System.Collections.Generic;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QSeekSafeCondition : Conditional
    {
        private SharedInt _state;
        private SharedGameObject _target;
        private Dictionary<int, float> utilitys = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            _target = Owner.GetVariable("NearSafe") as SharedGameObject;
            float[][] qTable = GameMgr.Instance.qTable;
            for (int i = 0; i < 1024; ++i)
            {
                if (qTable[i][(int)ActionSpace.SeekSafe] == qTable[i].Max())
                    utilitys.Add(i, qTable[i][(int)ActionSpace.SeekSafe]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_target.Value != null && utilitys.ContainsKey(_state.Value))
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            if (utilitys.ContainsKey(_state.Value))
                return utilitys[_state.Value];
            return 0;
        }
    }
}
