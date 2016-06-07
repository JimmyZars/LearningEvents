using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
        class xmlEvent
        {
            public string name, location;
            public string daytime;
            public int hour, min;
            public xmltime starttime, endtime;

            public xmlEvent()
            {
                //Empty
            }

            public xmlEvent(string name, xmltime starttime, xmltime endtime, string location)
            {
                this.name = name;
                this.starttime = starttime;
                this.endtime = endtime;
                this.location = location;
            }

            public void setName(string name)
            {
                this.name = name;
            }
        }
        class xmltime
        {
            private int hour, min;
            private string daytime;

            public xmltime()
            {
                //blank constructor
            }

            public xmltime(int hour, int min, string daytime)
            {
                this.hour = hour;
                this.min = min;
                this.daytime = daytime;
            }

            public void setHour(int hr)
            {
                this.hour = hr;
            }

            public void setMin(int mins)
            {
                this.min = mins;
            }

            public void setDayTime(string dt)
            {
                this.daytime = dt;
            }

            public string getHour()
            {
                if (this.hour < 10)
                {
                    return "0" + this.hour;
                }
                else {
                    return "" + this.hour;
                }
            }
            public string getMin()
            {
                if (this.min < 10)
                {
                    return "0" + this.min;
                }
                else {
                    return "" + this.min;
                }
            }

            public string getDayTime()
            {
                return this.daytime;
            }

        }
    }

