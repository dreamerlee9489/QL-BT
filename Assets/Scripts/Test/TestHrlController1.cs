using BehaviorDesigner.Runtime;
using UnityEngine;

namespace App
{
	public class TestHrlController1 : MonoBehaviour
	{
        private int _hp, _tem, _cnt;
        private SharedInt _currentState;
        private BehaviorTree bt;

        private void Awake()
        {
            bt = GetComponent<BehaviorTree>();
            _currentState = bt.GetVariable("CurrentState") as SharedInt;
        }

        private void Start()
        {
            _hp = 1;
            _tem = Random.Range(0, 2);
            _cnt = Random.Range(0, 2);
            _currentState.Value = (_hp << 2) | (_tem << 1) | _cnt;
        }
    }
}