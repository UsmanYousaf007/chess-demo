using System;
using Random = UnityEngine.Random;

namespace HUF.Utils.MinMaxRange
{
    [Serializable]
    public class MinMaxInt : MinMaxRange<int>
    {
        public MinMaxInt()
        {
        }

        public MinMaxInt(MinMaxInt other)
        {
            min = other.min;
            max = other.max;
        }
        
        public MinMaxInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
        
        public override int GetRandomValue()
        {
            return Random.Range(min, max + 1);
        }

        public override string ToString()
        {
            return $"Min: {min:0}; Max: {max:0};";
        }
    }
}

