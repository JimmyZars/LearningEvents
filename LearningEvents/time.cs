using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningEvents
{
    class time
    {
        public int hour, min;
        public string daytime;

        public time()
        {
            //blank constructor
        }

        public time(int hour, int min, string daytime)
        {
            this.hour = hour;
            this.min = min;
            this.daytime = daytime;
        }
    }
}
