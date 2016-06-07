using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningEvents
{
    class learningEventsCourse
    {
        private string courseName;
        private string courseCode;

        public learningEventsCourse()
        {
            courseName = "";
            courseCode = "";
        }

        public learningEventsCourse(string cName, string cCode){
            courseCode = cCode;
            courseName = cName;
        }

        public string getName()
        {
            return courseName;
        }

        public string getCode()
        {
            return courseCode;
        }

        public string getCodeByName(string cName)
        {
            return courseCode;
        }
    }
}
