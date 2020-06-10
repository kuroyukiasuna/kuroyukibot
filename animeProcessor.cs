using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using libAniDB.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kuroyukibot
{
   public class animeProcessor
    {
        private string animeName { get; set; }
        private string animeName_CHS { get; set; }
        private string anilist_id { get; set; }
        private string similarity { get; set; }
        private string episode { get; set; }
        private bool is_R18 { get; set; }
        
        public string searchImage(string img)
        {
            moveImage(img);
            getAnime(img);

            return composeMessage();
        }

        private void moveImage(string img)
        {
            string path1 = @"d:/装机/酷Q Pro/data/image/" + img;
            string path2 = @"./kuroyukiDB/animeImage/" + img;

            if (File.Exists(path2))
            {
                File.Delete(path2);
            }

            File.Move(path1, path2);
        }
        private string composeMessage()
        {
            string result = "动画名称： ";
            if(animeName_CHS == "")
            {
                result += animeName;
            }
            else
            {
                result += animeName_CHS;
            }

            result += "\n";

            if (episode != "")
            {
                result += "该画面出自第" + episode + "话\n";
            }

            
            result += "匹配相似度： " + similarity + "\n" +
                      "请以相似度大于90%为准，若相似度低于90%则结果仅供参考";

            if (is_R18)
            {
                result = "注意！ 该作品为R18动画\n" + result;
            }
            else
            {
                result += "\nhttp://anilist.co/anime/" + anilist_id;
            }

            return result;
        }

        private void getAnime(string img)
        {

            byte[] imageArray = System.IO.File.ReadAllBytes(@"./kuroyukiDB/animeImage/" + img);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);


            var request = (HttpWebRequest)WebRequest.Create("https://trace.moe/api/search");

            var postData = "image="+base64ImageRepresentation;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //dynamic result = JObject.Parse(responseString);

            var output = JsonConvert.DeserializeObject(responseString);
            JToken result = null;

            foreach (KeyValuePair<string, JToken> item in (JObject)output)
            {
                if (item.Key == "docs")
                {
                    result = item.Value;
                    break;
                }
            }

            animeName  = result.First["anime"].ToString();
            animeName_CHS = result.First["title_chinese"].ToString();
            anilist_id = result.First["anilist_id"].ToString();
            episode = result.First["episode"].ToString();
            similarity = result.First["similarity"].ToString().Substring(2,2) + "%";

            if(result.First["is_adult"].ToString() == "False")
            {
                is_R18 = false;
            }
            else
            {
                is_R18 = true;
            }

            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://trace.moe/api/search");
            //httpWebRequest.Method = "POST";
            //httpWebRequest.ContentType = "application/json";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{

            //    string json = "{\"image\" : "+ base64ImageRepresentation +"}";

            //    streamWriter.Write(json);
            //    streamWriter.Flush();
            //}

            //var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    Console.WriteLine(result);
            //}

        }
    }
}
