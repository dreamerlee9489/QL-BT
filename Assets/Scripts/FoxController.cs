using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
	public class FoxController : MonoBehaviour
	{
        public int hp = 100;
        public TMP_Text text;

        private BehaviorTree _bt;
        public SharedGameObject nearRabbit;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            nearRabbit = _bt.GetVariable("NearRabbit") as SharedGameObject;
            _bt.DisableBehavior();
        }

        private void Start()
        {
            _bt.RestartWhenComplete = true;
            _bt.EnableBehavior();
        }

        public void GetDemage(int demage, RabbitController enemy)
        {
            hp = Math.Max(hp - demage, 0);
            text.text = hp.ToString();
            if (hp == 0)
            {
                enemy.nearFox.Value = null;
                GameMgr.Instance.FoxIsDead(GetComponent<NavMeshAgent>());
            }
        }
    }
}
