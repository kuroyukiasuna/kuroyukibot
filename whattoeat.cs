using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MyCommonHelper.FileHelper;

namespace kuroyukibot
{
    public class whattoeat
    {
        private DateTime cooldown;

        class GroupMeals : IComparable<GroupMeals>
        {
            public GroupMeals()
            {
                Meals = new List<string>();
            }
            public List<string> Meals { get; set; }

            public long GroupId { get; set; }

            public string MealListName { get; set; }

            public int CompareTo(GroupMeals obj)
            {
                if (obj == null) return 1;
                GroupMeals otherFruit = obj as GroupMeals;
                if (GroupId > otherFruit.GroupId) { return 1; }
                else
                {
                    if (GroupId == otherFruit.GroupId) { return 0; }
                    else { return -1; }
                }
            }
        }

        List<GroupMeals> groupMealsDb { get; set; }

        void initialize()
        {   
            cooldown = DateTime.Now;
           
        }

        public whattoeat()
        {
            groupMealsDb = new List<GroupMeals>();
        }

        public void initializeDB()
        {
            string p = @"./kuroyukiDB/listDB.csv";
            //read the json 
            //var sourceContent = File.ReadAllText(p);
            //parse as array  
            // var sourceobjects = JArray.Parse("[" + sourceContent + "]");
            //JObject source = JObject.Parse(sourceContent);
            //GroupMeals jp = (GroupMeals)JsonConvert.DeserializeObject(sourceContent);//result为上面的Json数据

            CsvFileHelper myCsv = new CsvFileHelper(p, Encoding.UTF8);
            var myData = myCsv.GetListCsvData();
            long GID = 0;
            string MLN = "";

            foreach (List<string> ele in myData)
            {


                if (ele[0] != "")
                {
                    GID = long.Parse(ele[0]);
                }
                else if (ele[1] != "")
                {
                    MLN = ele[1];
                }
                else
                {
                    GroupMeals temp = new GroupMeals();
                    temp.GroupId = GID;
                    temp.MealListName = MLN;
                    foreach (string elem in ele)
                    {
                        if (elem != "")
                        {
                            temp.Meals.Add(elem);
                        }

                    }
                    groupMealsDb.Add(temp);
                }
            }
            groupMealsDb.Sort();
            //其他的操作
            myCsv.Dispose();
            //updateFile();
        }

        public void updateFile()
        {

            string p = @"./kuroyukiDB/listDB.csv";
            groupMealsDb.Sort();
            List<List<string>> mydata = new List<List<string>>();

            long record = 0;

            foreach (GroupMeals ele in groupMealsDb)
            {
                if (ele.GroupId != record)
                {
                    record = ele.GroupId;
                    List<string> row1 = new List<string>();
                    row1.Add(ele.GroupId.ToString());
                    mydata.Add(row1);
                }

                List<string> row2 = new List<string>();
                row2.Add("");
                row2.Add(ele.MealListName);
                mydata.Add(row2);

                List<string> row3 = new List<string>();
                row3.Add("");
                row3.Add("");
                foreach(string elem in ele.Meals)
                {
                    row3.Add(elem);
                }
                mydata.Add(row3);
                
            }
            
            CsvFileHelper.SaveCsvFile(p, mydata, false, new System.Text.UTF8Encoding(true));
        }

        private void CreateJson()
        {
            JObject source = new JObject();
            source.Add("Name", "yanzhiyi");

            string p = @"./kuroyukiDB/Create.json";
            //found the file exist 
            if (!File.Exists(p))
            {
                FileStream fs1 = new FileStream(p, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            //write the json to file 
            File.WriteAllText(p, source.ToString());
        }

        public string randResult(long groupId, string mealListName)
        {
            string result = "";
            foreach (GroupMeals ele in groupMealsDb)
            {
                if (ele.GroupId == groupId && ele.MealListName == mealListName)
                {
                    if (ele.Meals.Count() == 1 && ele.Meals[0] == "empty")
                    {
                        result = "该菜单为空菜单";
                    }
                    else
                    {
                        result = "吃" + ele.Meals[(new Random()).Next(ele.Meals.Count())] + "怎么样？";
                    }
                }
            }
            if (result.Length == 0)
            {
                result = "该群不存在指定菜单";
            }
                    return result;
        }

        public string addLst(long groupId, string mealListName)
        {
            int i = -99;
            bool flag = false;
            string result;

            foreach(GroupMeals ele in groupMealsDb)
            {
                if(ele.GroupId == groupId)
                {
                    i++;
                }

                if(ele.MealListName == mealListName && ele.GroupId == groupId)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                result = "该菜单已存在";
            }else if(i >= 3)
            {
                result = "该群菜单数量已满";
            }
            else
            {
                GroupMeals temp = new GroupMeals();
                temp.GroupId = groupId;
                temp.MealListName = mealListName;
                temp.Meals.Add("empty");
                groupMealsDb.Add(temp);
                updateFile();
                result = "菜单已添加";
            }

            return result;
        }

        public string removeLst(long groupId, string mealListName)
        {
            int i = 0;
            bool flag = false;

            string result;

            foreach (GroupMeals ele in groupMealsDb)
            {

                if (ele.GroupId == groupId && ele.MealListName == mealListName)
                {
                    flag = true;
                    break;
                }
                i++;
            }

            if (flag)
            {
                result = "已移除菜单";
                groupMealsDb.RemoveAt(i);
                updateFile();

            }
            else
            {
                result = "未发现该菜单";
            }

            return result;
        }

            public string addMeal(long groupId, string mealListName, string mealName)
        {
            bool flag = false;
            bool flag2 = false;
            string result;

            foreach (GroupMeals ele in groupMealsDb)
            {
                if(ele.GroupId == groupId && ele.MealListName == mealListName)
                {
                    if(ele.Meals[0] == "empty")
                    {
                        ele.Meals.RemoveAt(0);
                    }

                    foreach(string elem in ele.Meals)
                    {
                        if(elem == mealName)
                        {
                            flag2 = true;
                        }
                    }

                    if (!flag2)
                    {
                        ele.Meals.Add(mealName);
                        flag = true;
                    }
                    break;
                }
            }

            if (flag)
            {
                result = "选项已添加";
                updateFile();
            }
            else if (flag2)
            {
                result = "该选项已存在";
            }
            else
            {
                result = "未找到该菜单";
            }

            return result;
        }

        public string removeMeal(long groupId, string mealListName, string mealName)
        {
            bool flag = false;
            bool flag2 = false;
            string result;

            foreach (GroupMeals ele in groupMealsDb)
            {
                if (ele.GroupId == groupId && ele.MealListName == mealListName)
                {
                    flag = true;
                    foreach (string elem in ele.Meals)
                    {
                        if (elem == mealName)
                        {
                            ele.Meals.Remove(elem);
                            if (ele.Meals.Count() == 0)
                            {
                                ele.Meals.Add("empty");
                            }
                            flag2 = true;
                            break;
                        }
                    }

                    break;
                }
            }

            if (flag2)
            {
                result = "选项已移除";
                updateFile();
            }
            else if (!flag)
            {
                result = "未找到该菜单";
            }
            else
            {
                result = "未找到该选项";
            }

            return result;
        }

        public string showlst(long groupId)
        {
            string result = "";

            foreach (GroupMeals ele in groupMealsDb)
            {
                if(groupId == ele.GroupId)
                {
                    result += "\n" + ele.MealListName + "：   ";
                    if (ele.Meals.Count() == 1 && ele.Meals[0] == "empty")
                    {
                        result += "0个选项";
                    }
                    else
                    {
                        result += ele.Meals.Count().ToString() + "个选项";
                    }
                }
            }

            if(result.Length == 0)
            {
                result = "该群无可用菜单, 使用/wte addlst添加";
            }
            else
            {
                result =  "该群有如下菜单" + result;
            }
            return result;
        }

        public string show(long groupId, string mealListName)
        {
            string result = "";

            foreach (GroupMeals ele in groupMealsDb)
            {
                if (groupId == ele.GroupId && ele.MealListName == mealListName)
                {
                    if (ele.Meals.Count() == 1 && ele.Meals[0] == "empty")
                    {
                        result += "\n空菜单";
                    }
                    else
                    {
                        foreach (string elem in ele.Meals)
                        {
                            result += "\n" + (ele.Meals.IndexOf(elem) + 1).ToString() + ". " + elem;
                        }
                    }
                   break;
                }
                
            }

            if (result.Length == 0)
            {
                result = "该群不存在指定菜单, 使用/wte addlst添加";
            }
            else
            {
                result = mealListName +"有如下选项:" + result;
            }
            return result;
        }

        public string removeMealAt(long groupId, string mealListName, int at)
        {
            bool flag = false;
            bool flag2 = false;
            string result;

            foreach (GroupMeals ele in groupMealsDb)
            {
                if (ele.GroupId == groupId && ele.MealListName == mealListName)
                {
                    flag = true;
                    if (at < ele.Meals.Count() && at >= 0)
                    {
                        flag2 = true;
                        ele.Meals.RemoveAt(at);
                        if(ele.Meals.Count() == 0)
                        {
                            ele.Meals.Add("empty");
                        }
                    }
                    break;
                }
            }

            if (flag2)
            {
                result = "选项已移除";
                updateFile();
            }
            else if (!flag)
            {
                result = "未找到该菜单";
            }
            else
            {
                result = "未找到该选项";
            }

            return result;
        }

        public string GenerateMessage(string message, long groupId)
        {
            //string midtime = (DateTime.Now - cooldown).ToString();
            //return midtime;

            string[] msgary = message.Split(" ");
            string result = "";

            if (msgary.Length >= 2)
            {
                if (msgary[1] == "addlst")
                {
                    if (msgary.Length == 3)
                    {
                        result = addLst(groupId, msgary[2]);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte addlst listname";
                    }
                }else if(msgary[1] == "add") {
                    if (msgary.Length == 4 && msgary[3] != "")
                    {
                        result = addMeal(groupId, msgary[2], msgary[3]);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte add listname choice";
                    }
                }else if(msgary[1] == "rmlst")
                {
                    if (msgary.Length == 3)
                    {
                        result = removeLst(groupId, msgary[2]);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte rmlst listname";
                    }
                }
                else if (msgary[1] == "rm")
                {
                    if (msgary.Length == 4 && msgary[3] != "")
                    {
                        result = removeMeal(groupId, msgary[2], msgary[3]);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte rm listname choice";
                    }
                }
                else if (msgary[1] == "rmat")
                {
                    if (msgary.Length == 4 && msgary[3] != "")
                    {
                        result = removeMealAt(groupId, msgary[2], int.Parse(msgary[3]) - 1);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte rmat listname choice";
                    }
                }
                else if (msgary[1] == "showlst")
                {
                    if (msgary.Length == 2)
                    {
                        result = showlst(groupId);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte showlst";
                    }
                }
                else if (msgary[1] == "show")
                {
                    if (msgary.Length == 3)
                    {
                        result = show(groupId, msgary[2]);
                    }
                    else
                    {
                        result = "消息格式错误, 应为/wte show listname";
                    }
                }
                else if (msgary[1] == "help")
                {
                    result = "WTE(What To Eat)命令列表:\n" +
                        "/wte help 或 /whattoeat - 打开帮助列表\n" +
                        "/wte 菜单名 - 从指定菜单随机\n" +
                        "/wte showlst - 展示群内所有菜单\n" +
                        "/wte show 菜单名 - 展示指定的菜单\n" +
                        "/wte addlst 菜单名 - 添加指定菜单, 每个群最多存在3个菜单, 不然蛾姐的内存顶不住= =！\n" +
                        "/wte rmlst 菜单名 - 移除指定菜单\n" +
                        "/wte add 菜单名 菜品名 - 添加菜名到指定菜单\n" +
                        "/wte rm 菜单名 菜品名 - 移除指定菜单里的指定菜品\n" +
                        "/wte rmat 菜单名 菜品编号 - 移除指定菜单里的指定编号";
                }
                else
                {

                    if (msgary.Length == 2)
                    {
                        result = randResult(groupId, msgary[1]);
                    }
                    else
                    {
                        result = "命令错误，请使用/wte help查询";
                    }
                }
            }
            else
            {
                result = "命令错误，请使用/wte help查询";
            }

            return result;

        }

       
    }
}
