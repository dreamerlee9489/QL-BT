using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
    public class RabbitController : MonoBehaviour
    {
        public int hp = 100;
        public bool withinSafe = false;
        public TMP_Text text;

        private BehaviorTree _bt;
        private SharedGameObject _nearFood, _nearSafe, _nearFox;
        private SharedFloat _arriveDist, _viewDist, _neighDist, _fleeCD, _eatCD;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox, _state;

        private static GameObject[] _allFood, _allSafe, _rabbits, _foxs;
        public static GameObject[] Rabbits => _rabbits;
        public static GameObject[] Foxs => _foxs;


        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
            _nearFox = _bt.GetVariable("NearFox") as SharedGameObject;
            _nearFood = _bt.GetVariable("NearFood") as SharedGameObject;
            _nearSafe = _bt.GetVariable("NearSafe") as SharedGameObject;
            _heathLv = _bt.GetVariable("HealthLevel") as SharedInt;
            _neighNum = _bt.GetVariable("NeighNum") as SharedInt;
            _distFood = _bt.GetVariable("DistFood") as SharedInt;
            _distSafe = _bt.GetVariable("DistSafe") as SharedInt;
            _distFox = _bt.GetVariable("DistFox") as SharedInt;
            _state = _bt.GetVariable("State") as SharedInt;
            _arriveDist = _bt.GetVariable("ArriveDist") as SharedFloat;
            _viewDist = _bt.GetVariable("ViewDist") as SharedFloat;
            _neighDist = _bt.GetVariable("NeighDist") as SharedFloat;
            _fleeCD = _bt.GetVariable("FleeCD") as SharedFloat;
            _eatCD = _bt.GetVariable("EatCD") as SharedFloat;
            _bt.DisableBehavior();
        }

        private void Start()
        {
            GetComponent<NavMeshAgent>().isStopped = false;
            _allFood ??= GameObject.FindGameObjectsWithTag("FoodPos");
            _allSafe ??= GameObject.FindGameObjectsWithTag("SafePos");
            _rabbits ??= GameObject.FindGameObjectsWithTag("Rabbit");
            _foxs ??= GameObject.FindGameObjectsWithTag("Fox");
            _bt.RestartWhenComplete = true;
            _bt.EnableBehavior();
        }

        private void FixedUpdate()
        {
            if (_fleeCD.Value > 0)
                _fleeCD.Value = Math.Max(_fleeCD.Value - Time.deltaTime, 0);
            if (_eatCD.Value > 0)
                _eatCD.Value = Math.Max(_eatCD.Value - Time.deltaTime, 0);
            _heathLv.Value = (int)GetHealthLevel();
            _neighNum.Value = (int)GetNeighbourNum();
            _distFood.Value = (int)GetDistanceToFood();
            _distSafe.Value = (int)GetDistanceToSafe();
            _distFox.Value = (int)GetDistanceToFox();
            _state.Value = 0;
            _state.Value = (_heathLv.Value << 8) | (_neighNum.Value << 6) | (_distFood.Value << 4) | (_distSafe.Value << 2) | _distFox.Value;
        }

        public bool CanBeSee()
        {
            if (!gameObject.activeSelf || withinSafe)
                return false;
            return true;
        }

        public void GetDemage(int demage, GameObject enemy = null)
        {
            _nearFox.Value = enemy;
            hp = Math.Max(hp - demage, 0);
            text.text = hp.ToString();
            if (hp == 0)
                GameMgr.Instance.RabbitIsDead(GetComponent<NavMeshAgent>());
        }

        public HealthLevel GetHealthLevel()
        {
            if (hp <= 0)
                return HealthLevel.None;
            else if (hp <= 35)
                return HealthLevel.Low;
            else if (hp <= 70)
                return HealthLevel.Medium;
            return HealthLevel.High;
        }

        public NeighbourNum GetNeighbourNum()
        {
            int count = 0;
            foreach (var rabbit in _rabbits)
                if (rabbit != gameObject && rabbit.activeSelf && Vector3.Distance(transform.position, rabbit.transform.position) <= _neighDist.Value)
                    ++count;
            if (count < 2)
                return NeighbourNum.None;
            else if (count < 4)
                return NeighbourNum.Low;
            else if (count < 6)
                return NeighbourNum.Medium;
            return NeighbourNum.High;
        }

        public DistanceToPos GetDistanceToFood()
        {
            float distance = float.MaxValue;
            if (_nearFood.Value != null)
                distance = Vector3.Distance(_nearFood.Value.transform.position, transform.position);
            else
            {
                GameObject nearFood = null;
                foreach (var item in _allFood)
                {
                    float tmp = Vector3.Distance(item.transform.position, transform.position);
                    if (tmp < distance)
                    {
                        nearFood = item;
                        distance = tmp;
                    }
                }
                _nearFood.Value = distance <= _viewDist.Value ? nearFood : null;
            }
            if (distance <= _arriveDist.Value)
                return DistanceToPos.Inside;
            else if (distance <= _viewDist.Value)
                return DistanceToPos.Near;
            else if (distance <= _neighDist.Value)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToSafe()
        {
            float distance = float.MaxValue;
            if (_nearSafe.Value != null)
                distance = Vector3.Distance(_nearSafe.Value.transform.position, transform.position);
            else
            {
                GameObject nearSafe = null;
                foreach (var item in _allSafe)
                {
                    float tmp = Vector3.Distance(item.transform.position, transform.position);
                    if (tmp < distance)
                    {
                        nearSafe = item;
                        distance = tmp;
                    }
                }
                _nearSafe.Value = distance <= _viewDist.Value ? nearSafe : null;
            }
            if (distance <= _arriveDist.Value)
                return DistanceToPos.Inside;
            else if (distance <= _viewDist.Value)
                return DistanceToPos.Near;
            else if (distance <= _neighDist.Value)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToFox()
        {
            float distance = float.MaxValue;
            if (_nearFox.Value != null && _nearFox.Value.activeSelf)
                distance = Vector3.Distance(_nearFox.Value.transform.position, transform.position);
            else
            {
                GameObject nearFox = null;
                foreach (var fox in _foxs)
                {
                    if (fox.activeSelf)
                    {
                        float tmp = Vector3.Distance(fox.transform.position, transform.position);
                        if (tmp < distance)
                        {
                            nearFox = fox;
                            distance = tmp;
                        }
                    }
                }
                _nearFox.Value = distance <= _viewDist.Value ? nearFox : null;
            }
            if (distance <= _arriveDist.Value)
                return DistanceToPos.Inside;
            else if (distance <= _viewDist.Value)
                return DistanceToPos.Near;
            else if (distance <= _neighDist.Value)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }
    }
}
