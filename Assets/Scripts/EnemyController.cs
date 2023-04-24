using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
	public class EnemyController : MonoBehaviour
	{
        public int hp = 100;
        public float viewRange = 30;
        public TMP_Text text;

        private BehaviorTree _bt;
        public static GameObject[] players, enemies;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            players ??= GameObject.FindGameObjectsWithTag("Player");
            enemies ??= GameObject.FindGameObjectsWithTag("Enemy");
            _bt.DisableBehavior();
        }

        private void Start()
        {
            _bt.EnableBehavior();
        }

        public void GetDemage(int demage, GameObject enemy)
        {
            hp = Math.Max(hp - demage, 0);
            text.text = hp.ToString();
            _bt.SetVariableValue("Target", enemy);
            if (hp == 0)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                gameObject.SetActive(false);
                _bt.DisableBehavior();
            }
            print("enemy get dmg: " + demage);
        }
    }
}
