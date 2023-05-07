using BehaviorDesigner.Runtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace App
{
    public class RabbitController : MonoBehaviour
    {
        private BehaviorTree _bt;
        private SharedFloat _arriveDist, _viewDist, _neighDist, _fleeCD, _eatCD;
        private SharedInt _heathLv, _neighNum, _distFood, _distSafe, _distFox, _state;
        private SharedGameObject _nearFood, _nearSafe, _nearFox;
        private TMP_Text _hpText, _goalText;

        public int hp = 100;
        public bool withinSafe = false;
        public SharedGameObject NearFood => _nearFood;
        public SharedGameObject NearSafe => _nearSafe;
        public SharedGameObject NearFox => _nearFox;
        public TMP_Text HpText => _hpText;
        public TMP_Text GoalText => _goalText;

        private void Awake()
        {
            _bt = GetComponent<BehaviorTree>();
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
            _nearFox = _bt.GetVariable("NearFox") as SharedGameObject;
            _nearFood = _bt.GetVariable("NearFood") as SharedGameObject;
            _nearSafe = _bt.GetVariable("NearSafe") as SharedGameObject;
            _hpText = transform.Find("Canvas").Find("HpText").GetComponent<TMP_Text>();
            _goalText = transform.Find("Canvas").Find("GoalText").GetComponent<TMP_Text>();
            _bt.DisableBehavior();
        }

        private void Start()
        {
            _heathLv.Value = (int)GetHealthLevel();
            _neighNum.Value = (int)GetNeighbourNum();
            _distFood.Value = (int)GetDistanceToFood();
            _distSafe.Value = (int)GetDistanceToSafe();
            _distFox.Value = (int)GetDistanceToFox();
            _state.Value = 0;
            _state.Value = (_heathLv.Value << 8) | (_neighNum.Value << 6) | (_distFood.Value << 4) | (_distSafe.Value << 2) | _distFox.Value;
            _goalText.text = "";
            _bt.RestartWhenComplete = true;
            _bt.EnableBehavior();
            GetComponent<NavMeshAgent>().isStopped = false;
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

        private void LateUpdate()
        {
            _hpText.transform.parent.forward = Camera.main.transform.forward;
        }

        public bool CanBeSee()
        {
            if (!gameObject.activeSelf || withinSafe)
                return false;
            return true;
        }

        public void GetDemage(int demage, FoxController enemy = null)
        {
            hp = Math.Max(hp - demage, 0);
            _hpText.text = hp.ToString();
            if (hp == 0)
            {
                enemy.NearRabbit.Value = null;
                GameMgr.Instance.RabbitIsDead(GetComponent<NavMeshAgent>());
            }
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
            foreach (var rabbit in GameMgr.Instance.Rabbits)
                if (rabbit != gameObject && rabbit.GetComponent<RabbitController>().CanBeSee() 
                    && Vector3.Distance(transform.position, rabbit.transform.position) <= _neighDist.Value)
                    ++count;
            if (count < 3)
                return NeighbourNum.None;
            else if (count < 5)
                return NeighbourNum.Low;
            else if (count < 7)
                return NeighbourNum.Medium;
            return NeighbourNum.High;
        }

        public DistanceToPos GetDistanceToFood()
        {
            GameObject nearFood = null;
            float distance = float.MaxValue;
            foreach (var food in GameMgr.Instance.Foods)
            {
                float tmp = Vector3.Distance(food.transform.position, transform.position);
                if (tmp < distance)
                {
                    nearFood = food;
                    distance = tmp;
                }
            }
            this._nearFood.Value = distance <= _neighDist.Value ? nearFood : null;
            if (distance <= _arriveDist.Value)
                return DistanceToPos.Inside;
            else if (distance <= 30)
                return DistanceToPos.Near;
            else if (distance <= 60)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToSafe()
        {
            GameObject nearSafe = null;
            float distance = float.MaxValue;
            foreach (var safe in GameMgr.Instance.Safes)
            {
                float tmp = Vector3.Distance(safe.transform.position, transform.position);
                if (tmp < distance)
                {
                    nearSafe = safe;
                    distance = tmp;
                }
            }
            this._nearSafe.Value = distance <= _neighDist.Value ? nearSafe : null;
            if (distance <= _arriveDist.Value)
                return DistanceToPos.Inside;
            else if (distance <= 50)
                return DistanceToPos.Near;
            else if (distance <= 100)
                return DistanceToPos.Medium;
            return DistanceToPos.Far;
        }

        public DistanceToPos GetDistanceToFox()
        {
            float distance = float.MaxValue;
            if (_nearFox.Value != null && _nearFox.Value.activeSelf)
            {
                distance = Vector3.Distance(_nearFox.Value.transform.position, transform.position);
                if (distance > _viewDist.Value)
                    _nearFox.Value = null;
            }
            if (_nearFox.Value == null)
            {
                GameObject nearFox = null;
                foreach (var fox in GameMgr.Instance.Foxs)
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
                this._nearFox.Value = distance <= _viewDist.Value ? nearFox : null;
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
