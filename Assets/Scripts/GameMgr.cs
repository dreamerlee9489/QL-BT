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
        private int _round, _liveRabbitNum, _safeRabbitNum, _liveFoxNum;
        private float _avgRabbitHp, _avgFoxHp, _timer;
        private static GameMgr _instance = null;
        private static List<GameObject> _foods = new(), _safes = new(), _rabbits = new(), _foxs = new();

        public bool isQL;
        public int roundNum, foxNum, rabbitNum;
        public GameObject foxObj, rabbitObj;
        public Text roundText, liveRabbitText, safeRabbitText, liveFoxText;
        public List<List<float>> qTable = new();

        public static List<GameObject> Rabbits => _rabbits;
        public static List<GameObject> Foxs => _foxs;
        public static List<GameObject> Foods => _foods;
        public static List<GameObject> Safes => _safes;
        public static GameMgr Instance => _instance;

        private void Awake()
        {
            _instance = this;
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
            _foods = GameObject.FindGameObjectsWithTag("FoodPos").ToList();
            _safes = GameObject.FindGameObjectsWithTag("SafePos").ToList();
            ResetGame();
        }

        private void Update()
        {
            if (!_gameOver && _round <= roundNum)
            {
                _timer += Time.deltaTime;
                if (_timer >= 180.0f)
                    RecordGame(isQL, "Time Over");
            }
        }

        private void ResetGame()
        {
            foreach (var rabbit in _rabbits)
                Destroy(rabbit);
            foreach (var fox in _foxs)
                Destroy(fox);
            _rabbits.Clear();
            _foxs.Clear();
            if (++_round <= roundNum)
            {
                for (int i = 0; i < rabbitNum; i++)
                {
                    GameObject rabbit = Instantiate(rabbitObj, new Vector3(Random.Range(-250.0f, 250.0f), 1, Random.Range(-250.0f, 250.0f)), Quaternion.identity);
                    rabbit.name = "Rabbit " + i;
                    _rabbits.Add(rabbit);
                }
                for (int i = 0; i < foxNum; i++)
                {
                    GameObject fox = Instantiate(foxObj, new Vector3(Random.Range(-250.0f, 250.0f), 1, Random.Range(-250.0f, 250.0f)), Quaternion.identity);
                    fox.name = "Fox " + i;
                    _foxs.Add(fox);
                }
                _liveRabbitNum = rabbitNum;
                _liveFoxNum = foxNum;
                _safeRabbitNum = 0;
                _avgRabbitHp = 0;
                _avgFoxHp = 0;
                _timer = 0;
                roundText.text = _round.ToString();
                liveRabbitText.text = _liveRabbitNum.ToString();
                safeRabbitText.text = _safeRabbitNum.ToString();
                liveFoxText.text = _liveFoxNum.ToString();
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
            agent.isStopped = true;
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            agent.GetComponent<RabbitController>().withinSafe = true;
            safeRabbitText.text = _safeRabbitNum.ToString();           
        }

        public void RabbitIsDead(NavMeshAgent agent)
        {
            if (--_liveRabbitNum == 0)
                RecordGame(isQL, "All Rabbits is dead");
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            liveRabbitText.text = _liveRabbitNum.ToString();
        }

        public void FoxIsDead(NavMeshAgent agent)
        {
            if (--_liveFoxNum == 0)
                RecordGame(isQL, "All Foxs is dead");
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            liveFoxText.text = _liveFoxNum.ToString();
        }
    }
}
