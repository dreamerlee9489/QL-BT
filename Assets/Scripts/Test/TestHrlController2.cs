using BehaviorDesigner.Runtime;
using UnityEngine;

namespace App
{
    public class TestHrlController2 : MonoBehaviour
	{
        private int _hp, _nn, _df, _ds, _de;
        private SharedInt _currentState;
        private BehaviorTree bt;

        private void Awake()
        {
            bt = GetComponent<BehaviorTree>();
            _currentState = bt.GetVariable("CurrentState") as SharedInt;
        }

        private void Start()
        {
            _hp = 3;
            _nn = Random.Range(0, 4);
            _df = Random.Range(0, 4);
            _ds = Random.Range(0, 4);
            _de = Random.Range(0, 4);
            _currentState.Value = (_hp << 8) | (_nn << 6) | (_df << 4) | (_ds << 2) | _de;
        }
    }
}
