using System;
using Random = UnityEngine.Random;

namespace HUF.Utils.MinMaxRange
{
    [Serializable]
    public class MinMaxFloat : MinMaxRange<float>
    {
        public MinMaxFloat()
        {
        }

        public MinMaxFloat(MinMaxFloat other)
        {
            min = other.min;
            max = other.max;
        }
        
        public MinMaxFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        
        public override float GetRandomValue()
        {
            return Random.Range(min, max);
        }

        public override string ToString()
        {
            return $"Min: {min:0.00}; Max: {max:0.00};";
        }
    }
}

