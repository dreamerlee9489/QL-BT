using App;
using System.Collections.Generic;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QEatCondition : Conditional
    {
        private SharedFloat _eatCD;
        private SharedInt _state;
        private Dictionary<int, float> utilitys = new();

        public override void OnAwake()
        {
            _eatCD = Owner.GetVariable("EatCD") as SharedFloat;
            _state = Owner.GetVariable("State") as SharedInt;
            float[][] qTable = GameMgr.Instance.qTable;
            for (int i = 0; i < 1024; ++i)
            {
                if (qTable[i][(int)ActionSpace.Eat] == qTable[i].Max())
                    utilitys.Add(i, qTable[i][(int)ActionSpace.Eat]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_eatCD.Value == 0 && utilitys.ContainsKey(_state.Value))
            {
                _eatCD.Value = 4;
                return TaskStatus.Success;
            }
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
