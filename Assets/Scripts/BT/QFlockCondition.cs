using App;
using System.Collections.Generic;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class QFlockCondition : Conditional
    {
        private SharedInt _state;
        private Dictionary<int, float> utilitys = new();

        public override void OnAwake()
        {
            _state = Owner.GetVariable("State") as SharedInt;
            float[][] qTable = GameMgr.Instance.qTable;
            for (int i = 0; i < 1024; ++i)
            {
                if (qTable[i][(int)ActionSpace.Flock] == qTable[i].Max())
                    utilitys.Add(i, qTable[i][(int)ActionSpace.Flock]);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (utilitys.ContainsKey(_state.Value))
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
