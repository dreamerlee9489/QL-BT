using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App
{
	public class TestQLBrain : MonoBehaviour
	{
        int action_num = 2;
        int state_num = 2;
        double gamma = 0.8;
        float timer = 0, cd = 3, startTime;
        double[][] Q = null, R = null;
        private BehaviorTree bt;

        public SharedInt hp = 100;

        private void Awake()
        {
            Q = new double[state_num][];
            for (int i = 0; i < state_num; i++)
                Q[i] = new double[action_num];
            R = RewardTable(state_num, action_num);
            bt = GetComponent<BehaviorTree>();
            hp = bt.GetVariable("HP") as SharedInt;
            startTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - startTime < 5)
            {
                for (int s = 0; s < state_num; s++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int sp = ExplorationPolicy(R[s]/*, epsilon*/);
                        // 找到下一个状态的最大Q值
                        double q_max = Q[sp].Max();
                        // Bellman optimal iteration equation
                        Q[s][sp] = R[s][sp] + gamma * q_max;
                        s = sp;
                    }
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > cd)
                {
                    timer = 0;
                    hp.Value = Math.Max(hp.Value - 10, 0);
                    if (hp.Value == 0)
                        hp.Value = 100;
                }
            }
        }

        private void OnApplicationQuit()
        {
            PrintArray(Q);
        }

        double[][] RewardTable(int state_num, int action_num)
        {
            double[][] R = new double[state_num][];
            for (int i = 0; i < state_num; i++)
                R[i] = new double[action_num];
            for (int i = 0; i < state_num; i++)
                for (int j = 0; j < action_num; j++)
                    R[i][j] = -1;
            R[0][1] = 10;
            R[1][0] = 10;
            return R;
        }

        int ExplorationPolicy(double[] V, double epsilon = 0.5)
        {
            List<int> index_list = new();
            for (int i = 0; i < V.Length; i++)
                if (V[i] >= 0) // 非负值表示可能的转移，压入状态索引
                    index_list.Add(i);
            return index_list[UnityEngine.Random.Range(0, index_list.Count)];
        }

        void PrintArray(double[][] arr)
        {
            StringBuilder builder = new("qTable\n");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                    builder.Append(arr[i][j] + "\t");
                builder.Append("\n");
            }
            Debug.Log(builder.ToString());
        }
    }
}
