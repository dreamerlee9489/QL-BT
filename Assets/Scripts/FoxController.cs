using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
	public class FoxController : MonoBehaviour
	{
        private BehaviorTree _bt;
        private SharedGameObject _nearRabbit;
        private ParticleSystem _chunk;
        private TMP_Text _text;

        public int hp = 100;
        public SharedGameObject NearRabbit => _nearRabbit;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            _nearRabbit = _bt.GetVariable("NearRabbit") as SharedGameObject;
            _text = transform.Find("Canvas").Find("HpText").GetComponent<TMP_Text>();
            _chunk = Resources.Load<ParticleSystem>("Chunk");
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
            hp = Math.Clamp(hp - demage, 0, 100);
            _text.text = hp.ToString();
            if (hp == 0)
            {
                enemy.NearFox.Value = null;
                Instantiate(_chunk, transform.position, transform.rotation).Play();
                GameMgr.Instance.FoxIsDead(GetComponent<NavMeshAgent>());
            }
        }
    }
}
