using System;

namespace HUF.Utils.Runtime.Extensions
{
    public static class EnumExtensions
    {
        public static bool EnumHasFlag(this Enum enumValue, Enum flag)
        {
            var int64Enum = Convert.ToInt64((object) enumValue);
            var int64Flag = Convert.ToInt64((object) flag);
            return (int64Enum & int64Flag) == int64Flag;
        }
    }
}