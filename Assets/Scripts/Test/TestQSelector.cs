using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}UtilitySelectorIcon.png")]
    public class TestQSelector : Composite, IRewarder
    {
        protected int currentChildIndex, _prevState;
        protected float[][] qTable, rTable;
        protected SharedInt currentState;
        protected TaskStatus executionStatus = TaskStatus.Inactive;
        protected List<int> availableChildren = new();

        public SharedFloat alpha = 0.5f, gamma = 0.8f;
        public SharedInt stateNum = 16, actionNum = 2;

        public override void OnAwake()
        {
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
            qTable = new float[stateNum.Value][];
            rTable = new float[stateNum.Value][];
            for (int i = 0; i < stateNum.Value; i++)
            {
                qTable[i] = new float[actionNum.Value];
                rTable[i] = new float[actionNum.Value];
            }
        }

        public override void OnStart()
        {
            availableChildren.Clear();
            for (int i = 0; i < children.Count; ++i)
                availableChildren.Add(i);
            currentChildIndex = ChooseAction();
        }

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override void OnChildStarted(int childIndex)
        {
            executionStatus = TaskStatus.Running;
        }

        public override bool CanExecute()
        {
            if (executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Running)
                return false;
            return availableChildren.Count > 0;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running)
            {
                UpdateQValue();
                executionStatus = childStatus;
                if (executionStatus == TaskStatus.Failure)
                {
                    availableChildren.Remove(childIndex);
                    if (availableChildren.Count > 0)
                        currentChildIndex = ChooseAction();
                }                
            }
        }

        public override void OnConditionalAbort(int childIndex)
        {
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
            PrintArray(qTable, $"Q_{FriendlyName}");
            PrintArray(rTable, $"R_{FriendlyName}");
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            return executionStatus;
        }

        public override bool CanRunParallelChildren()
        {
            return true;
        }

        protected int ChooseAction()
        {
            _prevState = currentState.Value;
            //Debug.Log("cnt=" +  availableChildren.Count);
            return availableChildren[Random.Range(0, availableChildren.Count)];
        }

        protected void UpdateQValue()
        {
            float reward = (children[currentChildIndex] as IRewarder).GetReward(_prevState);
            rTable[_prevState][currentChildIndex] = reward;
            qTable[_prevState][currentChildIndex] *= 1 - alpha.Value;
            qTable[_prevState][currentChildIndex] += alpha.Value * (reward + gamma.Value * qTable[currentState.Value].Max());
        }

        public float GetReward(int state)
        {
            float highestReward = float.MinValue;
            for(int i =0; i < children.Count;i++)
                highestReward = Mathf.Max(highestReward, (children[i] as IRewarder).GetReward(_prevState));
            return highestReward;
        }

        void PrintArray(float[][] arr, string fileName)
        {
            StringBuilder builder = new("hp,tem,cnt,area,");
            for (int i = 0; i < children.Count; i++)
                builder.Append(children[i].FriendlyName + (i == children.Count - 1 ? "\n" : ","));
            for (int i = 0; i < stateNum.Value; i++)
            {
                builder.Append($"{(i & 0b1000) >> 3},");
                builder.Append($"{(i & 0b0100) >> 2},");
                builder.Append($"{(i & 0b0010) >> 1},");
                builder.Append($"{(i & 0b0001)},");
                for (int j = 0; j < actionNum.Value; j++)
                    builder.Append(qTable[i][j] + (j == actionNum.Value - 1 ? "\n" : ","));
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/HrlSelector/{fileName}.csv");
            writer.Write(builder.ToString());
        }
    }
}