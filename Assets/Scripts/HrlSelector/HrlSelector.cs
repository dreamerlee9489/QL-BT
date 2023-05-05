using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}UtilitySelectorIcon.png")]
    public class HrlSelector : Composite, IRewarder
    {
        protected int _currIndex = 0, _prevState;
        protected float[][] _qTable, _rTable;
        protected TaskStatus executionStatus = TaskStatus.Inactive;
        protected List<int> availableChildren = new();
        protected SharedInt currentState;

        public SharedFloat alpha = 0.5f, gamma = 0.8f;
        public SharedInt stateNum = 1024, actionNum = 2;

        public override void OnAwake()
        {
            _qTable = new float[stateNum.Value][];
            _rTable = new float[stateNum.Value][];
            for (int i = 0; i < stateNum.Value; i++)
            {
                _qTable[i] = new float[actionNum.Value];
                _rTable[i] = new float[actionNum.Value];
            }
            currentState = Owner.GetVariable("CurrentState") as SharedInt;
        }

        public override void OnStart()
        {
            availableChildren.Clear();
            for (int i = 0; i < children.Count; ++i)
                availableChildren.Add(i);
            _currIndex = ChooseAction();
        }

        public override int CurrentChildIndex()
        {
            return _currIndex;
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
                UpdateQTable();
                executionStatus = childStatus;
                if (executionStatus == TaskStatus.Failure)
                {
                    availableChildren.Remove(childIndex);
                    _currIndex = ChooseAction();
                }
            }
        }

        public override void OnConditionalAbort(int childIndex)
        {
            _currIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            executionStatus = TaskStatus.Inactive;
            _currIndex = 0;
            PrintArray(_qTable, FriendlyName);
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
            for (int i = 0; i < _rTable[_prevState].Length; i++)
                _rTable[_prevState][i] = 0;
            return availableChildren[Random.Range(0, availableChildren.Count)];
        }

        protected void UpdateQTable()
        {
            float reward = (children[_currIndex] as IRewarder).GetReward(_prevState);
            _rTable[_prevState][_currIndex] = reward;
            _qTable[_prevState][_currIndex] *= (1 - alpha.Value);
            _qTable[_prevState][_currIndex] += alpha.Value * (reward + gamma.Value * _qTable[currentState.Value].Max());
            //Debug.Log($"{FriendlyName}: Q[{_prevState}][{currentChildIndex}]={q}");
        }

        public float GetReward(int state) => _rTable[_prevState].Min();

        void PrintArray(float[][] arr, string fileName)
        {          
            StringBuilder builder = new("Health,NeighNum,DistFood,DistSafe,DistFox,");
            for (int i = 0; i < children.Count; i++)
                builder.Append(children[i].FriendlyName + (i == children.Count - 1 ? "\n" : ","));
            for (int i = 0; i < stateNum.Value; i++)
            {
                builder.Append($"{(i & 0b1100000000) >> 8},");
                builder.Append($"{(i & 0b0011000000) >> 6},");
                builder.Append($"{(i & 0b0000110000) >> 4},");
                builder.Append($"{(i & 0b0000001100) >> 2},");
                builder.Append($"{(i & 0b0000000011)},");
                for (int j = 0; j < actionNum.Value; j++)
                    builder.Append(arr[i][j] + (j == actionNum.Value - 1 ? "\n" : ","));
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/HrlSelector/{fileName}.csv");
            writer.Write(builder.ToString());
        }
    }
}