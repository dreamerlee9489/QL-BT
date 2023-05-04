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
        protected int currentChildIndex = 0;
        protected float highestReward;
        protected bool reevaluating;
        protected float[][] qTable, rTable;
        protected float[] rewards;
        protected TaskStatus executionStatus = TaskStatus.Inactive;
        protected List<int> availableChildren = new();
        protected SharedInt previouState, currentState;

        public SharedFloat alpha = 0.5f, gamma = 0.8f;
        public SharedInt stateNum = 8, actionNum = 2;

        public override void OnAwake()
        {
            qTable = new float[stateNum.Value][];
            rTable = new float[stateNum.Value][];
            rewards = new float[actionNum.Value];
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
            availableChildren.Clear();
            for (int i = 0; i < children.Count; ++i)
            {
                availableChildren.Add(i);
                rewards[i] = 0;
            }
            currentChildIndex = ChooseAction(currentState.Value);
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
            if (executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Running || reevaluating)
                return false;
            return availableChildren.Count > 0;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running)
            {
                UpdateQTable((children[currentChildIndex] as IRewarder).GetReward(previouState.Value));
                executionStatus = childStatus;
                if (executionStatus == TaskStatus.Failure)
                {
                    availableChildren.Remove(childIndex);
                    currentChildIndex = ChooseAction(currentState.Value);
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
            PrintArray(qTable);
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            return executionStatus;
        }

        public override bool CanRunParallelChildren()
        {
            return true;
        }

        public float GetReward(int state)
        {
            highestReward = float.MinValue;
            for (int i = 0; i < availableChildren.Count; ++i)
            {
                float reward = (children[availableChildren[i]] as IRewarder).GetReward(state);
                if (reward > highestReward)
                    highestReward = reward;
            }
            return highestReward;
        }

        protected void UpdateQTable(float reward)
        {
            if (reward < 0)
                rTable[previouState.Value][currentChildIndex] = reward;
            rewards[currentChildIndex] = reward;
            float q = (1 - alpha.Value) * qTable[previouState.Value][currentChildIndex] + alpha.Value * (reward + gamma.Value * qTable[currentState.Value].Max());
            qTable[previouState.Value][currentChildIndex] = q;
            Debug.Log($"{FriendlyName}: Q[{previouState.Value}][{currentChildIndex}]={q}");
        }

        protected int ChooseAction(int state)
        {
            return availableChildren[Random.Range(0, availableChildren.Count)];
        }

        void PrintArray(float[][] arr)
        {
            StringBuilder builder = new("prevHp,prevTem,prevCnt,a1,a2\n");
            for (int i = 0; i < stateNum.Value; i++)
            {
                builder.Append($"{(i & 0b100) >> 2},");
                builder.Append($"{(i & 0b010) >> 1},");
                builder.Append($"{(i & 0b001)},");
                for (int j = 0; j < actionNum.Value; j++)
                    builder.Append(arr[i][j] + ",");
                builder.Append("\n");
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/QLSelector/{FriendlyName}.csv");
            writer.Write(builder.ToString());
        }
    }
}