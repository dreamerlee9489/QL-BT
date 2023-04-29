using BehaviorDesigner.Runtime;
using System.IO;
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
        [HideInInspector]
        public int liveRabbitNum, safeRabbitNum, liveFoxNum;
        [HideInInspector]
        public float avgRabbitHp, avgFoxHp, timer;
        [HideInInspector]
        public float[][] qTable;

        public bool isQL;
        public int foxNum, rabbitNum;
        public GameObject foxObj, rabbitObj;
        public Text liveRabbitText, safeRabbitText, liveFoxText;

        private bool _gameOver = false;
        private static GameMgr _instance = null;

        public static GameMgr Instance => _instance;

        private void Awake()
        {
            _instance = this;
            for (int i = 0; i < rabbitNum; i++)
            {
                GameObject tmp = Instantiate(rabbitObj, new Vector3(Random.Range(-250.0f, 250.0f), 1, Random.Range(-250.0f, 250.0f)), Quaternion.identity);
                tmp.name = "Rabbit " + i;
            }
            for (int i = 0; i < foxNum; i++)
            {
                GameObject tmp = Instantiate(foxObj, new Vector3(Random.Range(-250.0f, 250.0f), 1, Random.Range(-250.0f, 250.0f)), Quaternion.identity);
                tmp.name = "Fox " + i;
            }
            liveRabbitNum = rabbitNum;
            liveFoxNum = foxNum;
            safeRabbitNum = 0;
            qTable = new float[1024][];
            for (int i = 0; i < 1024; i++)
                qTable[i] = new float[8];
            if (File.Exists(Application.streamingAssetsPath + "/QTable.csv"))
            {
                string[] line;
                using StreamReader reader = File.OpenText(Application.streamingAssetsPath + "/QTable.csv");
                reader.ReadLine();
                for (int i = 0; i < 1024; ++i)
                {
                    line = reader.ReadLine().Split(',');
                    for (int j = 0; j < 8; j++)
                        qTable[i][j] = float.Parse(line[j + 5]);
                }
            }
        }

        private void Update()
        {
            if (!_gameOver)
            {
                timer += Time.deltaTime;
                if (timer > 120.0f)
                {
                    RecordGame(isQL, "Time Over");
                    //ResetGame();
                }
            }
        }

        private void ResetGame()
        {
            liveRabbitNum = rabbitNum;
            liveFoxNum = foxNum;
            safeRabbitNum = 0;
            avgRabbitHp = 0;
            avgFoxHp = 0;
            timer = 0;
        }

        private void RecordGame(bool isQL, string info)
        {
            _gameOver = true;
            int sumRabbitHp = 0, sumFoxHp = 0;
            string fileName = isQL ? "QLGameRecord" : "GameRecord";
            foreach (var item in RabbitController.Rabbits)
                sumRabbitHp += item.GetComponent<RabbitController>().hp;
            foreach (var item in RabbitController.Foxs)
                sumFoxHp += item.GetComponent<FoxController>().hp;
            avgRabbitHp = sumRabbitHp * 1.0f / rabbitNum;
            avgFoxHp = sumFoxHp * 1.0f / foxNum;
            if (!File.Exists($"{Application.streamingAssetsPath}/{fileName}.csv"))
            {
                using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/{fileName}.csv");
                writer.WriteLine("AliveRabbitNum,SafeRabbitNum,AvgRabbitHp,AliveFoxNum,AvgFoxHp");
            }
            using (StreamWriter writer = File.AppendText($"{Application.streamingAssetsPath}/{fileName}.csv"))
            {
                writer.WriteLine($"{liveRabbitNum},{safeRabbitNum},{avgRabbitHp},{liveFoxNum},{avgFoxHp}");
            }
            Debug.Log("RecordGame: " + info);
        }

        public void RabbitEnterSafe(NavMeshAgent agent)
        {
            if (++safeRabbitNum == RabbitController.Rabbits.Length)
                RecordGame(isQL, "All Rabbits is safe");
            agent.isStopped = true;
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            agent.GetComponent<RabbitController>().withinSafe = true;
            safeRabbitText.text = safeRabbitNum.ToString();           
        }

        public void RabbitIsDead(NavMeshAgent agent)
        {
            if (--liveRabbitNum == 0)
                RecordGame(isQL, "All Rabbits is dead");
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            liveRabbitText.text = liveRabbitNum.ToString();
        }

        public void FoxIsDead(NavMeshAgent agent)
        {
            if (--liveFoxNum == 0)
                RecordGame(isQL, "All Foxs is dead");
            agent.isStopped = true;
            agent.enabled = false;
            agent.gameObject.SetActive(false);
            agent.GetComponent<BehaviorTree>().DisableBehavior();
            liveFoxText.text = liveFoxNum.ToString();
        }
    }
}
