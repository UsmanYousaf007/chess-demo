namespace HUF.Utils.MinMaxRange
{
    [System.Serializable]
    public class MinMaxRange<T>
    {
        public T min;
        public T max;

        public MinMaxRange()
        {
        }

        public MinMaxRange(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public virtual T GetRandomValue()
        {
            return min;
        }
    }
}

