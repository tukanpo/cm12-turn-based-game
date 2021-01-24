using System;
using System.Linq;

namespace App.Util
{
    public static class EnumUtil
    {
        static readonly Random Rand = new Random();

        public static T Random<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(c => Rand.Next())
                .FirstOrDefault();
        }
    }
}
