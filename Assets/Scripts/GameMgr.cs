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
    public enum BtType { HandMade, QL, HRL }

    public class GameMgr : MonoBehaviour
    {
        private bool _gameOver = false;
        private int _round, _liveRabbitNum, _safeRabbitNum, _liveFoxNum, _camIdx;
        private float _avgRabbitHp, _avgFoxHp, _gameTimer, _camTimer;
        private float[][] _qTable;
        private GameObject _foxObj, _rabbitObj;
        private Text _roundText, _liveRabbitText, _safeRabbitText, _liveFoxText;
        private List<GameObject> _foods = new(), _safes = new(), _rabbits = new(), _foxs = new();
        private static GameMgr _instance = null;

        public BtType btType;
        public int roundNum, rabbitNum, foxNum;

        public float[][] Q => _qTable;
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
            if (btType == BtType.QL)
                LoadQTable();
        }

        private void Start()
        {
            Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
            switch (btType)
            {
                case BtType.HandMade:
                    _rabbitObj = Resources.Load<GameObject>("Rabbit");
                    break;
                case BtType.QL:
                    _rabbitObj = Resources.Load<GameObject>("QRabbit");
                    break;
                case BtType.HRL:
                    _rabbitObj = Resources.Load<GameObject>("HrlRabbit");
                    break;
            }
            _foxObj = Resources.Load<GameObject>("Fox");
            ResetGame();
        }

        private void Update()
        {
            if (!_gameOver && _round <= roundNum)
            {
                _gameTimer += Time.deltaTime;
                _camTimer += Time.deltaTime;
                if (_gameTimer >= 120.0f)
                    RecordGame(btType, "Time Over");
                //if (_camTimer >= 15.0f)
                //{
                //    _camTimer = 0;
                //    _camIdx = -1;
                //    do _camIdx = Random.Range(0, _rabbits.Count);
                //    while (!_rabbits[_camIdx].activeSelf);
                //}
            }
        }

        private void LateUpdate()
        {
            if (!_gameOver)
                Camera.main.transform.position = new Vector3(0, 50, 0) + _rabbits[_camIdx].transform.position;
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
                for (int i = 0; i < rabbitNum; i++)
                {
                    GameObject rabbit = Instantiate(_rabbitObj, new Vector3(Random.Range(-500.0f / 2, 500.0f / 2), 1, Random.Range(-500.0f / 2, 500.0f / 2)), Quaternion.identity);
                    rabbit.name = "Rabbit_" + i;
                    _rabbits.Add(rabbit);
                }
                for (int i = 0; i < foxNum; i++)
                {
                    GameObject fox = Instantiate(_foxObj, new Vector3(Random.Range(-500.0f / 2, 500.0f / 2), 1, Random.Range(-500.0f / 2, 500.0f / 2)), Quaternion.identity);
                    fox.name = "Fox_" + i;
                    _foxs.Add(fox);
                }
                //_camIdx = Random.Range(0, _rabbits.Count);
                _camIdx = 50;
                _liveRabbitNum = _rabbits.Count;
                _liveFoxNum = _foxs.Count;
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

        private void RecordGame(BtType type, string info)
        {
            string fileName = "";
            switch (type)
            {
                case BtType.HandMade:
                    fileName = "HandMadeRecord";
                    break;
                case BtType.QL:
                    fileName = "QConditionRecord";
                    break;
                case BtType.HRL:
                    fileName = "HrlSelectorRecord";
                    break;
            }
            _gameOver = true;
            int sumRabbitHp = 0, sumFoxHp = 0;
            foreach (var rabbit in _rabbits)
                sumRabbitHp += rabbit.GetComponent<RabbitController>().hp;
            foreach (var fox in _foxs)
                sumFoxHp += fox.GetComponent<FoxController>().hp;
            _avgRabbitHp = sumRabbitHp * 1.0f / _rabbits.Count;
            _avgFoxHp = sumFoxHp * 1.0f / _foxs.Count;
            if (!File.Exists($"{Application.streamingAssetsPath}/{fileName}.csv"))
                using (StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/{fileName}.csv"))
                    writer.WriteLine("AliveRabbitNum,SafeRabbitNum,AvgRabbitHp,AliveFoxNum,AvgFoxHp");
            using (StreamWriter writer = File.AppendText($"{Application.streamingAssetsPath}/{fileName}.csv"))
                writer.WriteLine($"{_liveRabbitNum},{_safeRabbitNum},{_avgRabbitHp},{_liveFoxNum},{_avgFoxHp}");
            ResetGame();
            Debug.Log("RecordGame: " + info);
        }

        public void RabbitEnterSafe(NavMeshAgent agent)
        {
            agent.isStopped = true;
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            _safeRabbitText.text = (++_safeRabbitNum).ToString();
            if (_safeRabbitNum == _rabbits.Count)
                RecordGame(btType, "All Rabbits is safe");
        }

        public void RabbitIsDead(NavMeshAgent agent)
        {
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            _liveRabbitText.text = (--_liveRabbitNum).ToString();
            if (_liveRabbitNum == 0)
                RecordGame(btType, "All Rabbits is dead");
        }

        public void FoxIsDead(NavMeshAgent agent)
        {
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            _liveFoxText.text = (--_liveFoxNum).ToString();
            if (_liveFoxNum == 0)
                RecordGame(btType, "All Foxs is dead");
        }

        private void LoadQTable()
        {
            _qTable = new float[1024][];
            for (int i = 0; i < 1024; i++)
                _qTable[i] = new float[8];
            if (File.Exists(Application.streamingAssetsPath + "/QL/QTable.csv"))
            {
                string[] line;
                using StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/QL/QTable.csv");
                reader.ReadLine();
                for (int i = 0; i < 1024; ++i)
                {
                    line = reader.ReadLine().Split(',');
                    for (int j = 0; j < 8; j++)
                        _qTable[i][j] = float.Parse(line[j + 5]);
                }
            }
        }
    }
}
