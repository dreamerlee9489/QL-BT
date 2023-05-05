using App;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}UtilitySelectorIcon.png")]
    public class TestQSelector : Composite, IRewarder
    {
        protected int _epoch, _currIndex, _prevState;
        protected double _highestReward, _alpha, _epsilon;
        protected double[][] _qTable, _rTable;
        protected SharedInt _currState;
        protected TaskStatus _childStatus = TaskStatus.Inactive;
        protected List<int> _availableIndexs = new();
        protected Text _alphaTxt, _epochTxt, _epsilonTxt;

        public SharedInt stateNum = 16, actionNum = 2, epochNum = 1000;
        public SharedDouble gamma = 0.9f, alphaMax = 0.9f, alphaDecay = 0.005f;
        public SharedDouble epsilonMax = 1.0f, epsilonMin = 0.05f, epsilonDecay = 0.999f;

        public double[][] Q => _qTable;
        public double[][] R => _rTable;

        public override int CurrentChildIndex() => _currIndex;

        public override void OnChildStarted(int childIndex) => _childStatus = TaskStatus.Running;

        public override TaskStatus OverrideStatus(TaskStatus status) => _childStatus;

        public override bool CanRunParallelChildren() => true;

        public override bool CanExecute()
        {
            ++_epoch;
            _alpha = alphaMax.Value / (1 + alphaDecay.Value * _epoch);
            _epsilon = System.Math.Max(epsilonMin.Value, epsilonMax.Value * System.Math.Pow(epsilonDecay.Value, _epoch));
            if (ID == 1)
            {
                _alphaTxt.text = _alpha.ToString();
                _epochTxt.text = _epoch.ToString();
                _epsilonTxt.text = _epsilon.ToString();
            }

            if (_childStatus == TaskStatus.Success || _childStatus == TaskStatus.Running)
                return false;
            return _availableIndexs.Count > 0;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running)
            {
                UpdateQValue();
                _childStatus = childStatus;
                if (_childStatus == TaskStatus.Failure)
                {
                    _availableIndexs.Remove(childIndex);
                    int index = ChooseAction(_currState.Value, _epsilon);
                    if (index >= 0)
                        _currIndex = index;
                }
            }
        }

        public override void OnConditionalAbort(int childIndex)
        {
            _currIndex = childIndex;
            _childStatus = TaskStatus.Inactive;
        }

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("CurrentState") as SharedInt;
            _qTable = new double[stateNum.Value][];
            _rTable = new double[stateNum.Value][];
            for (int i = 0; i < stateNum.Value; i++)
            {
                _qTable[i] = new double[actionNum.Value];
                _rTable[i] = new double[actionNum.Value];
            }
            if (ID == 1)
            {
                _alphaTxt = Owner.GetComponent<TestHrlController1>().alphaTxt;
                _epochTxt = Owner.GetComponent<TestHrlController1>().epochTxt;
                _epsilonTxt = Owner.GetComponent<TestHrlController1>().epsilonTxt;
            }
        }

        public override void OnStart()
        {
            _availableIndexs.Clear();
            for (int i = 0; i < children.Count; ++i)
                _availableIndexs.Add(i);
            _currIndex = ChooseAction(_currState.Value, _epsilon);
        }

        public override void OnEnd()
        {
            _currIndex = 0;
            _childStatus = TaskStatus.Inactive;
            if (ID == 1)
            {
                PrintArray(_qTable, $"{FriendlyName}");
                foreach (var item in children)
                {
                    TestQSelector child = item as TestQSelector;
                    child.PrintArray(child.Q, child.FriendlyName);
                }
            }
        }

        protected int ChooseAction(int state, double epsilon)
        {
            if (_availableIndexs.Count == 0)
                return -1;
            _prevState = _currState.Value;
            if (Random.Range(0.0f, 1.0f) < epsilon)
                return _availableIndexs[Random.Range(0, _availableIndexs.Count)];
            else
            {
                double max = _qTable[state].Max();
                if (max < 0)
                {
                    _availableIndexs.Clear();
                    _currIndex = 0;
                    _childStatus = TaskStatus.Success;
                    return -1;
                }
                return System.Array.IndexOf(_qTable[state], max);
            }
        }

        protected void UpdateQValue()
        {
            double reward = (children[_currIndex] as IRewarder).GetReward(_prevState);
            _rTable[_prevState][_currIndex] = reward;
            _qTable[_prevState][_currIndex] *= 1 - _alpha;
            _qTable[_prevState][_currIndex] += _alpha * (reward + gamma.Value * _qTable[_currState.Value].Max());
            //Debug.Log($"{FriendlyName}: Q[{_prevState}][{_currIndex}]={_qTable[_prevState][_currIndex]}");
        }

        public double GetReward(int state)
        {
            _highestReward = double.MinValue;
            for (int i = 0; i < children.Count; i++)
                _highestReward = System.Math.Max(_highestReward, (children[i] as IRewarder).GetReward(_prevState));
            return _highestReward;
        }

        void PrintArray(double[][] arr, string fileName)
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
                    builder.Append(_qTable[i][j] + (j == actionNum.Value - 1 ? "\n" : ","));
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/HrlSelector/{fileName}.csv");
            writer.Write(builder.ToString());
        }
    }
}