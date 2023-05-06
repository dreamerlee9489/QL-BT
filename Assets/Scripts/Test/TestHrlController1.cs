using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
	public class TestHrlController1 : MonoBehaviour
	{
        private int _hp, _tem, _cnt, _area;
        private SharedInt _currentState;
        private BehaviorTree bt;

        public Text alphaTxt, epochTxt, epsilonTxt;

        private void Awake()
        {
            bt = GetComponent<BehaviorTree>();
            _currentState = bt.GetVariable("State") as SharedInt;
        }

        private void Start()
        {
            _hp = 1;
            _tem = Random.Range(0, 2);
            _cnt = Random.Range(0, 2);
            _area = Random.Range(0, 2);
            _currentState.Value = (_hp << 3) | (_tem << 2) | (_cnt << 1) | _area;
        }
    }
}
