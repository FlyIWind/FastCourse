using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeCourseFaster.Enity
{
    public class Course
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string RoomID { get; set; }
        public string RoomName { get; set; }
        public string Weeks { get; set; }
        public Object CourseTimes { get; set; }
    }
    public class CourseTime 
    {
        public int DayOfTheWeek { get; set; }
        public int TimeOfTheDay { get; set; }
    }
}
