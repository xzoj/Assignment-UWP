using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UWP_NTA_.Entity;

namespace UWP_NTA_.Service
{
    class ApiHandle
    {
        public static String API_REGISTER = "https://2-dot-backup-server-002.appspot.com/_api/v2/members";
        private static String API_LOGIN = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
        public static String API_CREATE_SONG = "http://2-dot-backup-server-002.appspot.com/_api/v2/songs";
        public static String API_MY_SONG = "http://2-dot-backup-server-002.appspot.com/_api/v2/songs/get-mine";

        //Xu ly dang nhap
        public async static Task<HttpResponseMessage> Sign_Up(Member member)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_REGISTER, content);
            return response.Result;
        }
        public static HttpResponseMessage Sign_In(string email, string password)
        {
            Dictionary<String, String> LoginInfor = new Dictionary<string, string>();
            LoginInfor.Add("email", email);
            LoginInfor.Add("password", password);

            HttpClient httpClient = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(LoginInfor), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_LOGIN, content);
            return response.Result;
        }
    }
}
