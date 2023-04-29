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
        private SharedGameObject _nearRabbit;
        public static GameObject[] rabbits, foxs;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            _nearRabbit = _bt.GetVariable("NearRabbit") as SharedGameObject;
            _bt.DisableBehavior();
        }

        private void Start()
        {
            rabbits ??= GameObject.FindGameObjectsWithTag("Rabbit");
            foxs ??= GameObject.FindGameObjectsWithTag("Fox");
            _bt.EnableBehavior();
        }

        public void GetDemage(int demage, GameObject enemy)
        {
            _nearRabbit.Value = enemy;
            hp = Math.Max(hp - demage, 0);
            text.text = hp.ToString();
            if (hp == 0)
                GameMgr.Instance.FoxIsDead(GetComponent<NavMeshAgent>());
        }
    }
}
