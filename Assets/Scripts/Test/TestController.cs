using BehaviorDesigner.Runtime;
using UnityEngine;

namespace App
{
    public class TestController : MonoBehaviour
	{
        private BehaviorTree bt;
        private SharedInt _hp, _tem, _cnt;
        private SharedInt _currentState;

        private void Awake()
        {
            bt = GetComponent<BehaviorTree>();
            _hp = bt.GetVariable("Hp") as SharedInt;
            _tem = bt.GetVariable("Tem") as SharedInt;
            _cnt = bt.GetVariable("Cnt") as SharedInt;
            _currentState = bt.GetVariable("CurrentState") as SharedInt;
        }
    }
}
