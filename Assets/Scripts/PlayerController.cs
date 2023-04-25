using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
    public enum HealthLevel { None, Low, Medium, High }
    public enum NeighbourNum { None, Low, Medium, High }
    public enum DistanceToPos { Inside, Near, Medium, Far }

    public class PlayerController : MonoBehaviour
    {
        public int hp = 100;
        public float viewRange = 30;
        public TMP_Text text;
        public SharedInt heathLevel, neighNum, distFood, distSafe, distEnemy, distNeigh;
        public static SharedGameObject foodPos, safePos;
        
        private BehaviorTree _bt;
        private SharedGameObject _target;
        private static GameObject[] _players, _enemies;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            _target = _bt.GetVariable("Target") as SharedGameObject;
            heathLevel = _bt.GetVariable("HealthLevel") as SharedInt;
            neighNum = _bt.GetVariable("NeighbourNum") as SharedInt;
            distFood = _bt.GetVariable("DistanceToFood") as SharedInt;
            distSafe = _bt.GetVariable("DistanceToSafe") as SharedInt;
            distEnemy = _bt.GetVariable("DistanceToTarget") as SharedInt;
            distNeigh = _bt.GetVariable("NeighbourDist") as SharedInt;
            _players ??= GameObject.FindGameObjectsWithTag("Player");
            _enemies ??= GameObject.FindGameObjectsWithTag("Enemy");
            _bt.DisableBehavior();
        }

        private void Start()
        {
            foodPos ??= GlobalVariables.Instance.GetVariable("FoodPos") as SharedGameObject;
            safePos ??= GlobalVariables.Instance.GetVariable("SafePos") as SharedGameObject;
            _bt.EnableBehavior();
        }

        private void FixedUpdate()
        {
            heathLevel.Value = (int)GetHealthLevel();
            neighNum.Value = (int)GetNeighbourNum();
            distFood.Value = (int)GetDistanceToFood();
            distSafe.Value = (int)GetDistanceToSafe();
            distEnemy.Value = (int)GetDistanceToTarget();
        }

        public void GetDemage(int demage, GameObject enemy = null)
        {
            hp = Math.Max(hp - demage, 0);
            text.text = hp.ToString();
            _bt.SetVariableValue("Target", enemy);
            if (hp == 0)
            {
                GetComponent<NavMeshAgent>().isStopped = true;
                GetComponent<NavMeshAgent>().enabled = false;
                gameObject.SetActive(false);
                _bt.DisableBehavior() ;
            }
        }

        public HealthLevel GetHealthLevel()
        {
            if (hp <= 0)
                return HealthLevel.None;
            else if (hp <= 30)
                return HealthLevel.Low;
            else if (hp <= 60)
                return HealthLevel.Medium;
            return HealthLevel.High;
        }

        public NeighbourNum GetNeighbourNum()
        {
            int count = 0;
            foreach (var player in _players)
                if (player != gameObject && player.activeSelf && Vector3.Distance(transform.position, player.transform.position) <= distNeigh.Value)
                    ++count;
            if (count == 0)
                return NeighbourNum.None;
            else if (count == 1)
                return NeighbourNum.Low;
            else if (count == 2)
                return NeighbourNum.Medium;
            return NeighbourNum.High;
        }

        public DistanceToPos GetDistanceToFood()
        {
            float distance = Vector3.Distance(foodPos.Value.transform.position, transform.position);
            if (distance <= 5)
                return DistanceToPos.Inside;
            else if (distance <= 20)
                return DistanceToPos.Near;
            else if (distance <= 40)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToSafe()
        {
            float distance = Vector3.Distance(safePos.Value.transform.position, transform.position);
            if (distance <= 5)
                return DistanceToPos.Inside;
            else if (distance <= 20)
                return DistanceToPos.Near;
            else if (distance <= 40)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToTarget()
        {
            float distance = 9999;
            if (_target.Value != null && _target.Value.activeSelf)
                distance = (_target.Value.transform.position - transform.position).magnitude;
            else
            {
                GameObject nearEnemy = null;
                foreach (var enemy in _enemies)
                {
                    if(enemy.activeSelf)
                    {
                        Vector3 dir = enemy.transform.position - transform.position;
                        if (dir.magnitude < distance)
                        {
                            distance = dir.magnitude;
                            nearEnemy = enemy;
                        }
                    }
                }
                _target.Value = distance <= viewRange ? nearEnemy : null;
            }
            if (distance <= 2)
                return DistanceToPos.Inside;
            else if (distance <= 10)
                return DistanceToPos.Near;
            else if (distance <= viewRange)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }
    }
}
