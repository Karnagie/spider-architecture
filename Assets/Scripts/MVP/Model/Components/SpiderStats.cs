using System;
using Data;

namespace MVP.Model
{
    public class SpiderStats
    {
        public IntStat Health;

        public SpiderStats()
        {
            Health = new IntStat(100);
        }
    }
}