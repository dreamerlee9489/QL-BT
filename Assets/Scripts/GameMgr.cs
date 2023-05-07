using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace App
{
    public enum HealthLevel { None, Low, Medium, High }
    public enum NeighbourNum { None, Low, Medium, High }
    public enum DistanceToPos { Inside, Near, Medium, Far }
    public enum ActionSpace { Flee, SeekSafe, SeekFood, Eat, Flock, Wander, Charge, Assist }

    public class GameMgr : MonoBehaviour
    {
        private bool _gameOver = false;
        private int _round, _liveRabbitNum, _safeRabbitNum, _liveFoxNum, _camIdx;
        private float _avgRabbitHp, _avgFoxHp, _gameTimer, _camTimer;
        private Text _roundText, _liveRabbitText, _safeRabbitText, _liveFoxText;
        private GameObject _hrlRabbit;
        private List<GameObject> _foods = new(), _safes = new(), _rabbits = new(), _foxs = new();
        private static GameMgr _instance = null;

        public bool isQL;
        public int roundNum, foxNum, rabbitNum;
        public GameObject foxObj, rabbitObj, hrlObj;
        public List<List<float>> qTable = new();

        public List<GameObject> Rabbits => _rabbits;
        public List<GameObject> Foxs => _foxs;
        public List<GameObject> Foods => _foods;
        public List<GameObject> Safes => _safes;
        public static GameMgr Instance => _instance;

        private void Awake()
        {
            _instance = this;
            _roundText = transform.Find("Canvas").Find("RoundNum").GetComponent<Text>();
            _liveRabbitText = transform.Find("Canvas").Find("AliveRabbitNum").GetComponent<Text>();
            _safeRabbitText = transform.Find("Canvas").Find("SafeRabbitNum").GetComponent<Text>();
            _liveFoxText = transform.Find("Canvas").Find("AliveFoxNum").GetComponent<Text>();
            _foods = GameObject.FindGameObjectsWithTag("FoodPos").ToList();
            _safes = GameObject.FindGameObjectsWithTag("SafePos").ToList();
            for (int i = 0; i < 1024; i++)
                qTable.Add(new List<float>());
            if (File.Exists(Application.streamingAssetsPath + "/QTable.csv"))
            {
                string[] line;
                using StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/QTable.csv");
                reader.ReadLine();
                for (int i = 0; i < 1024; ++i)
                {
                    line = reader.ReadLine().Split(',');
                    for (int j = 0; j < 8; j++)
                        qTable[i].Add(float.Parse(line[j + 5]));
                }
            }
        }

        private void Start()
        {
            _camIdx = 0;
            Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
            ResetGame();
        }

        private void Update()
        {
            if (!_gameOver && _round <= roundNum)
            {
                _gameTimer += Time.deltaTime;
                _camTimer += Time.deltaTime;
                if (_gameTimer >= 90.0f)
                    RecordGame(isQL, "Time Over");
                if (_hrlRabbit == null && _camTimer >= 15.0f)
                {
                    _camTimer = 0;
                    _camIdx = -1;
                    do _camIdx = Random.Range(0, _rabbits.Count);
                    while (!_rabbits[_camIdx].activeSelf);
                }
            }
        }

        private void LateUpdate()
        {
            if (!_gameOver)
                Camera.main.transform.position = new Vector3(0, 50, 0) + 
                    (_hrlRabbit != null ? _hrlRabbit.transform.position : _rabbits[_camIdx].transform.position);
        }

        private void ResetGame()
        {
            for (int i = 0; i < _rabbits.Count; i++)
                Destroy(_rabbits[i]);
            for (int i = 0; i < _foxs.Count; i++)
                Destroy(_foxs[i]);
            _rabbits.Clear();
            _foxs.Clear();
            if (++_round <= roundNum)
            {
                if (hrlObj != null)
                {
                    _hrlRabbit = Instantiate(hrlObj, new Vector3(Random.Range(-500.0f / 2, 500.0f / 2), 1, Random.Range(-500.0f / 2, 500.0f / 2)), Quaternion.identity);
                    _hrlRabbit.name = "HrlRabbit";
                    _rabbits.Add(_hrlRabbit);
                }
                for (int i = 0; i < rabbitNum; i++)
                {
                    GameObject rabbit = Instantiate(rabbitObj, new Vector3(Random.Range(-500.0f / 2, 500.0f / 2), 1, Random.Range(-500.0f / 2, 500.0f / 2)), Quaternion.identity);
                    rabbit.name = "Rabbit_" + i;
                    _rabbits.Add(rabbit);
                }
                for (int i = 0; i < foxNum; i++)
                {
                    GameObject fox = Instantiate(foxObj, new Vector3(Random.Range(-500.0f / 2, 500.0f / 2), 1, Random.Range(-500.0f / 2, 500.0f / 2)), Quaternion.identity);
                    fox.name = "Fox_" + i;
                    _foxs.Add(fox);
                }
                _camIdx = Random.Range(0, rabbitNum);
                _liveRabbitNum = rabbitNum;
                _liveFoxNum = foxNum;
                _safeRabbitNum = 0;
                _avgRabbitHp = 0;
                _avgFoxHp = 0;
                _gameTimer = 0;
                _roundText.text = _round.ToString();
                _liveRabbitText.text = _liveRabbitNum.ToString();
                _safeRabbitText.text = _safeRabbitNum.ToString();
                _liveFoxText.text = _liveFoxNum.ToString();
                _gameOver = false;
            }
        }

        private void RecordGame(bool isQL, string info)
        {
            _gameOver = true;
            int sumRabbitHp = 0, sumFoxHp = 0;
            string fileName = isQL ? "QLGameRecord" : "GameRecord";
            foreach (var item in _rabbits)
                sumRabbitHp += item.GetComponent<RabbitController>().hp;
            foreach (var item in _foxs)
                sumFoxHp += item.GetComponent<FoxController>().hp;
            _avgRabbitHp = sumRabbitHp * 1.0f / _rabbits.Count;
            _avgFoxHp = sumFoxHp * 1.0f / _foxs.Count;
            if (!File.Exists($"{Application.streamingAssetsPath}/{fileName}.csv"))
            {
                using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/{fileName}.csv");
                writer.WriteLine("AliveRabbitNum,SafeRabbitNum,AvgRabbitHp,AliveFoxNum,AvgFoxHp");
            }
            using (StreamWriter writer = File.AppendText($"{Application.streamingAssetsPath}/{fileName}.csv"))
            {
                writer.WriteLine($"{_liveRabbitNum},{_safeRabbitNum},{_avgRabbitHp},{_liveFoxNum},{_avgFoxHp}");
            }
            ResetGame();
            Debug.Log("RecordGame: " + info);
        }

        public void RabbitEnterSafe(NavMeshAgent agent)
        {
            if (++_safeRabbitNum == _rabbits.Count)
                RecordGame(isQL, "All Rabbits is safe");
            _safeRabbitText.text = _safeRabbitNum.ToString();
            agent.isStopped = true;
            agent.GetComponent<RabbitController>().withinSafe = true;
            agent.GetComponent<BehaviorTree>().DisableBehavior();
        }

        public void RabbitIsDead(NavMeshAgent agent)
        {
            if (--_liveRabbitNum == 0)
                RecordGame(isQL, "All Rabbits is dead");
            _liveRabbitText.text = _liveRabbitNum.ToString();
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
        }

        public void FoxIsDead(NavMeshAgent agent)
        {
            if (--_liveFoxNum == 0)
                RecordGame(isQL, "All Foxs is dead");
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            _liveFoxText.text = _liveFoxNum.ToString();
        }
    }
}
