using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}UtilitySelectorIcon.png")]
    public abstract class TestQSelector : Composite
    {
        protected float[][] qTable, rTable;
        protected int currentChildIndex = 0;
        protected SharedInt previouState, currentState;
        protected TaskStatus executionStatus = TaskStatus.Inactive;

        public SharedFloat alpha = 0.5f, gamma = 0.8f;
        public SharedInt stateNum = 8, actionNum = 2;

        public abstract float GetReward(int state);

        public override void OnAwake()
        {
            qTable = new float[stateNum.Value][];
            rTable = new float[stateNum.Value][];
            for (int i = 0; i < stateNum.Value; i++)
            {
                qTable[i] = new float[actionNum.Value];
                rTable[i] = new float[actionNum.Value];
            }
            previouState = Owner.GetVariable("PreviouState") as SharedInt;
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
        }

        public override void OnStart()
        {
            currentChildIndex = ChooseAction(currentState.Value);
        }

        public override void OnEnd()
        {
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
            PrintArray(qTable);
        }

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override bool CanExecute()
        {
            //Debug.Log($"{GetType().Name}: {currentAction}, {executionStatus}");
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Success;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            UpdateQTable(GetReward(previouState.Value));
            currentChildIndex = ChooseAction(currentState.Value);
            executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        protected int ChooseAction(int state)
        {
            List<int> actions = new();
            for (int i = 0; i < rTable[state].Length; i++)
                if (rTable[state][i] >= 0)
                    actions.Add(i);
            return actions[Random.Range(0, actions.Count)];
        }

        protected void UpdateQTable(float reward)
        {
            if (reward < 0)
                rTable[previouState.Value][currentChildIndex] = reward;
            float q = (1 - alpha.Value) * qTable[previouState.Value][currentChildIndex]
                + alpha.Value * (reward + gamma.Value * qTable[currentState.Value].Max());
            qTable[previouState.Value][currentChildIndex] = q;
            //Debug.Log($"{GetType().Name}: Q[{previouState.Value}][{currentAction}]");
        }

        void PrintArray(float[][] arr)
        {
            StringBuilder builder = new("hp,tem,cnt,a1,a2\n");
            for (int i = 0; i < stateNum.Value; i++)
            {
                if (arr[i].Max() > 0)
                {
                    builder.Append($"{(i & 0b110000) >> 4},");
                    builder.Append($"{(i & 0b001100) >> 2},");
                    builder.Append($"{(i & 0b000011)},");
                    for (int j = 0; j < actionNum.Value; j++)
                        builder.Append(arr[i][j] + ",");
                    builder.Append("\n");
                }
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/QLSelector/{GetType().Name}.csv");
            writer.Write(builder.ToString());
        }
    }
}