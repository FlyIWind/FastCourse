using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeCourseFaster.Enity;
using WeCourseFaster.Service;

namespace WeCourseFaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        
        [HttpGet]
        public string Get() {
            return "http://github.com/FlyIWind/";
        }
        [HttpPost("course")]
        public Result GetCourse(User user)
        {
            CookieContainer cookieContainer = new CookieContainer();
            var client = new RestClient("http://authserver.sict.edu.cn/authserver/login?service=http://szyjxgl.sict.edu.cn:9000/eams/");
            client.Timeout = -1;
            client.CookieContainer = cookieContainer;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            string lt_left = "<input type=\"hidden\" name=\"lt\" value=\"";
            string lt_right = "\"";
            string exe_left = "<input type=\"hidden\" name=\"execution\" value=\"";
            string exe_right = "\"";
            string salt_left = "<input type=\"hidden\" id=\"pwdDefaultEncryptSalt\" value=\"";
            string salt_right = "\"";
            string lt = Service.Encrypt.GetMiddleStr(response.Content, lt_left, lt_right);
            string exe = Service.Encrypt.GetMiddleStr(response.Content, exe_left, exe_right);
            string salt = Service.Encrypt.GetMiddleStr(response.Content, salt_left, salt_right);
            request = new RestRequest(Method.POST);
            request.AddParameter("username", user.UserName);
            request.AddParameter("password", Service.Encrypt.encryptAES(user.PassWord, salt));
            request.AddParameter("lt", lt);
            request.AddParameter("dllt", "userNamePasswordLogin");
            request.AddParameter("execution", exe);
            request.AddParameter("_eventId", "submit");
            request.AddParameter("rmShown", "1");
            response = client.Execute(request);
            Console.WriteLine(response.Content);
            Result result = new Result();
            if (response.Content.IndexOf("action") != -1)
            {
                result.code = 1;
                result.msg = "ok";
                client = new RestClient("http://szyjxgl.sict.edu.cn:9000/eams/localLogin.action");
                client.CookieContainer = cookieContainer;
                request = new RestRequest(Method.GET);
                client.Execute(request);
                client = new RestClient("http://szyjxgl.sict.edu.cn:9000/eams/courseTableForStd.action");
                client.CookieContainer = cookieContainer;
                response = client.Execute(request);
                string ids = Encrypt.GetMiddleStr(response.Content, "bg.form.addInput(form,\"ids\",\"","\"");
                client = new RestClient("http://szyjxgl.sict.edu.cn:9000/eams/courseTableForStd!courseTable.action");
                client.CookieContainer = cookieContainer;
                request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("ignoreHead", "1");
                request.AddParameter("setting.kind", "std");
                request.AddParameter("startWeek", "");
                request.AddParameter("semester.id", "30");
                request.AddParameter("ids", ids);
                response = client.Execute(request);
                string reg1 = "TaskActivity\\(actTeacherId.join\\(','\\),actTeacherName.join\\(','\\),\\\"(.*)\",\"(.*)\\(.*\\)\",\"(.*)\",\"(.*)\",\"(.*)\",null,null,assistantName,\"\",\"\"\\);((?:\\s*index =\\d+\\*unitCount\\+\\d+;\\s*.*\\s)+)";
                string reg2 = "\\s*index =(\\d+)\\*unitCount\\+(\\d+);\\s*";
                Regex regex = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(response.Content);
                regex = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                List<Course> courses = new List<Course>();
                foreach (Match match in matches) {
                    
                    Course course = new Course();
                    course.CourseID = match.Groups[1].Value;
                    course.CourseName = match.Groups[2].Value;
                    course.RoomID = match.Groups[3].Value;
                    course.RoomName = match.Groups[4].Value;
                    course.Weeks = match.Groups[5].Value;
                    System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
                    foreach (string indexStr in match.Groups[6].Value.Split("table0.activities[index][table0.activities[index].length]=activity;")) {
                        if (!indexStr.Contains("unitCount")) {
                            continue;
                        }
                        CourseTime courseTime = new CourseTime();
                        courseTime.DayOfTheWeek = int.Parse(regex.Match(indexStr).Groups[1].Value);
                        courseTime.TimeOfTheDay = int.Parse(regex.Match(indexStr).Groups[2].Value);
                        arrayList.Add(courseTime);
                    }
                    course.CourseTimes = arrayList.ToArray();
                    courses.Add(course);
                }
                result.data = courses;
            }
            else {
                result.code = 0;
                result.msg = "login fail";
            }
            return result;
        }

    }
}
