using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
	public class FoxController : MonoBehaviour
	{
        private TMP_Text _text;
        private BehaviorTree _bt;
        private SharedGameObject _nearRabbit;

        public int hp = 100;
        public SharedGameObject NearRabbit => _nearRabbit;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            _nearRabbit = _bt.GetVariable("NearRabbit") as SharedGameObject;
            _text = transform.Find("Canvas").Find("HpText").GetComponent<TMP_Text>();
            _bt.DisableBehavior();
        }

        private void Start()
        {
            _bt.RestartWhenComplete = true;
            _bt.EnableBehavior();
        }

        private void FixedUpdate()
        {
            _text.transform.parent.forward = Camera.main.transform.forward;
        }

        public void GetDemage(int demage, RabbitController enemy)
        {
            hp = Math.Max(hp - demage, 0);
            _text.text = hp.ToString();
            if (hp == 0)
            {
                enemy.NearFox.Value = null;
                GameMgr.Instance.FoxIsDead(GetComponent<NavMeshAgent>());
            }
        }
    }
}
