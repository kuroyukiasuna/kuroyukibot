using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MyCommonHelper.FileHelper;
using System.Linq;

namespace kuroyukibot
{
    public class CSVExtractor
    {
        List<List<string>> data;
        List<List<string>> dataCurrent;
        List<KeyValuePair<int, int>> toBeInserted;
        List<KeyValuePair<int, int>> toBeAdded;

        public CSVExtractor()
        {
            data = new List<List<string>>();
            dataCurrent = new List<List<string>>();
            toBeInserted = new List<KeyValuePair<int, int>>();
            toBeAdded = new List<KeyValuePair<int, int>>();
        }

        public string writeToDB(string message)
        {
            string p = @"./kuroyukiDB/recordDB.csv";
            string[] msgary = message.Split(" ");
            if (msgary[3] == "pic")
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

        private void sortOutColumns()
        {
            int i = 0;
            foreach(string title in dataCurrent[0])
            {
                if(title != "")
                {
                    insertToCol(i, dataCurrent[0].IndexOf(title));
                    i = dataCurrent[0].IndexOf(title);
                }
            }
            insertToCol(i, dataCurrent[0].Count());
        }

        private void insertToCol(int start, int end)
        {
            for(int i = start; i < end; i++)
            {
                if(dataCurrent[2][i] == "Final")
                {
                    insertToRow(i, dataCurrent[0][start]);
                }
            }
        }

        private void insertToRow(int start, string year)
        {
            if(data.Count() == 0)
            {
                List<string> temp = new List<string>();
                temp.Add("");
                temp.Add(year);
                temp.Add("");
                data.Add(temp);
                insertUnderColToRow(1, start, false);
            }
            else if (data[0].IndexOf(year) == -1)
            {
                data[0].Add(year);
                data[0].Add("");
                insertUnderColToRow(data[0].Count() - 2 , start, false);
            }
            else
            {
                int i;
                for(i = data[0].IndexOf(year) + 1; i < data[0].Count(); i++)
                {
                    if(data[0][i] != "")
                    {
                        break;
                    }
                }
                if (i == data[0].Count())
                {
                    data[0].Add("");
                    data[0].Add("");
                    insertUnderColToRow(i, start, false);
                }
                else
                {
                    data[0].Insert(i, "");
                    data[0].Insert(i, "");
                    insertUnderColToRow(i, start, true);
                }
                
            }
            return;
        }

        private void insertUnderColToRow(int startdata, int startcurrent, bool flag)
        {
            if (flag)
            {
                data[1].Insert(startdata, "");
                data[1].Insert(startdata, dataCurrent[1][startcurrent]);
                data[2].Add("Quantity");
                data[2].Add("Value");

                KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(startdata, startcurrent);
                toBeInserted.Add(tmp);
            }
            else
            {
                if(data.Count() == 1)
                {
                    List<string> temp = new List<string>();
                    temp.Add("");
                    temp.Add(dataCurrent[1][startcurrent]);
                    temp.Add("");
                    data.Add(temp);

                    temp = new List<string>();
                    temp.Add("C&A");
                    temp.Add("Quantity");
                    temp.Add("Value");
                    data.Add(temp);
                }
                else
                {
                    data[1].Add(dataCurrent[1][startcurrent]);
                    data[1].Add("");
                    data[2].Add("Quantity");
                    data[2].Add("Value");
                }

                KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(startdata, startcurrent);
                toBeAdded.Add(tmp);


            }
        }

        private int findrow(string col)
        {
            int result = -1;
            foreach (List<string> elem in data)
            {


                if(elem[0] == col)
                {
                    result = data.IndexOf(elem);
                }
            }
            return result;
        }

        private void theAddLoop()
        {
            int i = 0;
            foreach(List<string> ele in dataCurrent)
            {
                if (i < 4)
                {
                    i++;
                }
                else {
                    if (ele[0] != "")
                    {
                        int row = findrow(ele[0]);
                        foreach (KeyValuePair<int, int> elem in toBeAdded)
                        {
                            if (row == -1)
                            {
                                List<string> tmp2 = new List<string>();
                                tmp2.Add(ele[0]);
                                for (int j = 1; j < elem.Key; j++)
                                {
                                    tmp2.Add("");
                                }
                                tmp2.Add(ele[elem.Value]);
                                tmp2.Add(ele[elem.Value + 1]);
                                data.Add(tmp2);
                                row = data.Count() - 1;
                            }
                            else
                            {
                                data[row].Add(ele[elem.Value]);
                                data[row].Add(ele[elem.Value + 1]);
                            }
                        }

                        foreach (KeyValuePair<int, int> elem in toBeInserted)
                        {
                            if (row == -1)
                            {
                                List<string> tmp2 = new List<string>();
                                tmp2.Add(ele[0]);
                                for (int j = 1; j < elem.Key; j++)
                                {
                                    tmp2.Add("");
                                }
                                tmp2.Add(ele[elem.Value]);
                                tmp2.Add(ele[elem.Value + 1]);
                                data.Add(tmp2);
                                row = data.Count() - 1;
                            }
                            else
                            {
                                data[row].Insert(elem.Key, ele[elem.Value + 1]);
                                data[row].Insert(elem.Key, ele[elem.Value]);
                            }
                        }
                    }
                }
            }
        }

        private void sendWarningMessage()
        {

        }

        public void readToDataCurrent(string path)
        {
            for (int j = 1; j <= 21; j++)
            {
                string p = @"./kuroyukiDB/dataanalysis/"+ j + ".csv";
                dataCurrent.Clear();

                CsvFileHelper myCsv = new CsvFileHelper(p, Encoding.UTF8);
                var myData = myCsv.GetListCsvData();
                int i = 0;

                foreach (List<string> ele in myData)
                {
                    if (i == 6 || i == 8 || i == 9 || i == 11 || i >= 14)
                    {
                        dataCurrent.Add(ele);
                    }
                    i++;
                }
                //其他的操作
                myCsv.Dispose();
                sortOutColumns();
                theAddLoop();

                dataCurrent.Clear();
                toBeInserted.Clear();
                toBeAdded.Clear();
                dataCurrent = new List<List<string>>();
                toBeInserted = new List<KeyValuePair<int, int>>();
                toBeAdded = new List<KeyValuePair<int, int>>();
            }

            string p2 = @"./kuroyukiDB/dataanalysis/result.csv";
            CsvFileHelper.SaveCsvFile(p2, data, false, new System.Text.UTF8Encoding(true));
        }

    }
}
