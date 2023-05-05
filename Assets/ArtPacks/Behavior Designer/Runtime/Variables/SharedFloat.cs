namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedFloat : SharedVariable<float>
    {
        public static implicit operator SharedFloat(float value) { return new SharedFloat { Value = value }; }
    }

    [System.Serializable]
    public class SharedDouble : SharedVariable<double>
    {
        public static implicit operator SharedDouble(double value) { return new SharedDouble { Value = value }; }
    }
}