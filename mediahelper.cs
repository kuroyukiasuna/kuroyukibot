using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MyCommonHelper.FileHelper;
using System.Linq;

namespace kuroyukibot
{
    public class mediahelper
    {
        Dictionary<string, string> recordDB;
        Dictionary<string, string> imageDB;

        public mediahelper()
        {
            recordDB = new Dictionary<string, string>();
            imageDB = new Dictionary<string, string>();
        }

        public string writeToDB(string message)
        {
            string p = @"./kuroyukiDB/recordDB.csv";
            string[] msgary = message.Split(" ");
            if(msgary[3] == "pic")
            {
                p = @"./kuroyukiDB/imageDB.csv";
            }
            List<List<string>> mydata = new List<List<string>>();
            List<string> newRow = new List<string>();
            newRow.Add(msgary[1]);
            newRow.Add(msgary[2]);
            mydata.Add(newRow);

            CsvFileHelper.SaveCsvFile(p, mydata, false, new System.Text.UTF8Encoding(true));
            return "done";
        }

        public void initializeDB()
        {
            string p = @"./kuroyukiDB/recordDB.csv";

            CsvFileHelper myCsv = new CsvFileHelper(p, Encoding.UTF8);
            var myData = myCsv.GetListCsvData();

            foreach (List<string> ele in myData)
            {
                recordDB.Add(ele[0], ele[1]);
            }
            //其他的操作
            myCsv.Dispose();

            p = @"./kuroyukiDB/imageDB.csv";

            myCsv = new CsvFileHelper(p, Encoding.UTF8);
            myData = myCsv.GetListCsvData();

            foreach (List<string> ele in myData)
            {
                imageDB.Add(ele[0], ele[1]);
            }
            //其他的操作
            myCsv.Dispose();
        }
        public string sendRec(string key)
        {
            string val = recordDB[key];
            string result = "[CQ:record,file=kuroyukibot/"+val+"]";
            return result;
        }

        public string sendImage(string key)
        {
            string val = imageDB[key];
            string result = "[CQ:image,file=kuroyukibot/" + val + "]";
            return result;
        }
    }
}
