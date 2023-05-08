using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}UtilitySelectorIcon.png")]
    public class HrlSelector : Composite, IRewarder
    {
        protected int _epoch, _currIndex, _prevState;
        protected double _highestReward, _alpha, _epsilon;
        protected double[][] _qTable, _rTable;
        protected SharedInt _currState;
        protected TaskStatus _childStatus = TaskStatus.Inactive;
        protected List<int> _availableIndexs = new();
        protected Text _epochTxt, _alphaTxt, _epsilonTxt;

        public SharedInt stateNum = 1024, actionNum = 2, epochNum = 100000;
        public SharedDouble gamma = 0.9, alphaMax = 0.9, alphaDecay = 0.00005;
        public SharedDouble epsilonMax = 1.0, epsilonMin = 0.1, epsilonDecay = 0.9999;

        public int Epoch => _epoch;
        public double Alpha => _alpha;
        public double Epsilon => _epsilon;
        public double[][] Q => _qTable;
        public double[][] R => _rTable;

        public override int CurrentChildIndex() => _currIndex;

        public override void OnChildStarted(int childIndex) => _childStatus = TaskStatus.Running;

        public override TaskStatus OverrideStatus(TaskStatus status) => _childStatus;

        public override bool CanRunParallelChildren() => true;      

        public override void OnAwake()
        {
            _currState = Owner.GetVariable("State") as SharedInt;
            _qTable = new double[stateNum.Value][];
            _rTable = new double[stateNum.Value][];
            for (int i = 0; i < stateNum.Value; ++i)
            {
                _qTable[i] = new double[actionNum.Value];
                _rTable[i] = new double[actionNum.Value];
            }
            if (FriendlyName == "Root")
            {
                _epochTxt = transform.Find("RootCanvas").Find("EpochTxt").GetComponent<Text>();
                _alphaTxt = transform.Find("RootCanvas").Find("AlphaTxt").GetComponent<Text>();
                _epsilonTxt = transform.Find("RootCanvas").Find("EpsilonTxt").GetComponent<Text>();
            }
            LoadArray($"Q_{FriendlyName}.csv");
            ReadData($"Data_{FriendlyName}.txt");
        }

        public override void OnStart()
        {
            ++_epoch;
            _alpha = alphaMax.Value / (1.0 + alphaDecay.Value * _epoch);
            _epsilon = System.Math.Max(epsilonMin.Value, epsilonMax.Value * System.Math.Pow(epsilonDecay.Value, _epoch));
            if (ID == 1)
            {
                _alphaTxt.text = _alpha.ToString();
                _epochTxt.text = _epoch.ToString();
                _epsilonTxt.text = _epsilon.ToString();
            }

            _availableIndexs.Clear();
            for (int i = 0; i < children.Count; ++i)
                _availableIndexs.Add(i);
            _currIndex = ChooseAction(_currState.Value, _epsilon);
        }

        public override void OnEnd()
        {
            _currIndex = 0;
            _childStatus = TaskStatus.Inactive;
            if (FriendlyName == "Root")
            {
                PrintArray(_qTable, $"Q_{FriendlyName}.csv");
                WriteData(this, $"Data_{FriendlyName}.txt");
                for (int i = 0; i < children.Count; i++)
                {
                    HrlSelector child = children[i] as HrlSelector;
                    child.PrintArray(child.Q, $"Q_{child.FriendlyName}.csv");
                    child.WriteData(child, $"Data_{child.FriendlyName}.txt");
                    if (child.FriendlyName == "Idle")
                    {
                        foreach (var item in child.children)
                        {
                            HrlSelector tmp = item as HrlSelector;
                            tmp.PrintArray(tmp.Q, $"Q_{tmp.FriendlyName}.csv");
                            tmp.WriteData(tmp, $"Data_{tmp.FriendlyName}.txt");
                        }
                    }
                }
            }
        }

        public override bool CanExecute()
        {          
            if (_childStatus == TaskStatus.Success || _childStatus == TaskStatus.Running)
                return false;
            return _availableIndexs.Count > 0;
        }

        public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
        {
            if (childStatus != TaskStatus.Inactive && childStatus != TaskStatus.Running)
            {
                UpdateQTable(childIndex);
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
                if (max < -10)
                {
                    _currIndex = 0;
                    _childStatus = TaskStatus.Success;
                    return -1;
                }
                return System.Array.IndexOf(_qTable[state], max);
            }
        }

        protected void UpdateQTable(int action)
        {
            double reward = (children[action] as IRewarder).GetReward(_prevState);
            _rTable[_prevState][action] = reward;
            _qTable[_prevState][action] *= 1 - _alpha;
            _qTable[_prevState][action] += _alpha * (reward + gamma.Value * _qTable[_currState.Value].Max());
            //Debug.Log($"{FriendlyName}: Q[{_prevState}][{_currIndex}]={_qTable[_prevState][_currIndex]}");
        }

        public double GetReward(int state)
        {
            _highestReward = double.MinValue;
            for (int i = 0; i < children.Count; ++i)
                _highestReward = System.Math.Max(_highestReward, (children[i] as IRewarder).GetReward(_prevState));
            return _highestReward;
        }

        void PrintArray(double[][] arr, string fileName)
        {
            StringBuilder builder = new("Health,NeighNum,DistFood,DistSafe,DistFox,");
            for (int i = 0; i < children.Count; ++i)
                builder.Append(children[i].FriendlyName + (i < children.Count - 1 ? "," : "\n"));
            for (int i = 0; i < stateNum.Value; ++i)
            {
                builder.Append($"{(i & 0b1100000000) >> 8},");
                builder.Append($"{(i & 0b0011000000) >> 6},");
                builder.Append($"{(i & 0b0000110000) >> 4},");
                builder.Append($"{(i & 0b0000001100) >> 2},");
                builder.Append($"{(i & 0b0000000011)},");
                for (int j = 0; j < actionNum.Value; ++j)
                    builder.Append(arr[i][j] + (j < actionNum.Value - 1 ? "," : "\n"));
            }
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/HrlSelector/{fileName}");
            writer.Write(builder);
            writer.Flush();
        }

        void LoadArray(string fileName)
        {           
            if (File.Exists(Application.streamingAssetsPath + $"/HrlSelector/{fileName}"))
            {
                string[] line;
                using StreamReader reader = File.OpenText(Application.streamingAssetsPath + $"/HrlSelector/{fileName}");
                reader.ReadLine();
                for (int i = 0; i < stateNum.Value; ++i)
                {
                    line = reader.ReadLine().Split(',');
                    for (int j = 0; j < actionNum.Value; j++)
                        _qTable[i][j] = double.Parse(line[j + 5]);
                }
            }
        }

        void WriteData(HrlSelector node, string fileName)
        {
            string data = $"{node.Epoch},{node.Alpha},{node.Epsilon}";
            using StreamWriter writer = File.CreateText($"{Application.streamingAssetsPath}/HrlSelector/{fileName}");
            writer.Write(data);
            writer.Flush();
        }

        void ReadData(string fileName)
        {
            if (File.Exists(Application.streamingAssetsPath + $"/HrlSelector/{fileName}"))
            {
                using StreamReader reader = File.OpenText(Application.streamingAssetsPath + $"/HrlSelector/{fileName}");
                string[] line = reader.ReadLine().Split(',');
                _epoch = int.Parse(line[0]);
                _alpha = double.Parse(line[1]);
                _epsilon = double.Parse(line[2]);
            }
        }
    }
}