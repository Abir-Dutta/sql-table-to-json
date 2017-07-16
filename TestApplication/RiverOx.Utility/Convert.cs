using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RiverOx.Utility
{
    public static class Convert
    {
        /// <summary>
        /// Converts SQL DataTable to JSON format
        /// </summary>
        /// <param name="table">DataTable contains SQL Data.</param>
        /// <param name="primaryKey">Primary Key Name of the DataTable.</param>
        /// <returns>JSON String</returns>
        public static string SqlToJson(System.Data.DataTable table, string primaryKey)
        {
            var wholeResultset = table.Select().ToList();
            JArray finalResult = new JArray();

            if (wholeResultset.Count != 0)
            {
                Dictionary<string, JArray> arrayJsonObjectValuesForAllEntiry = new Dictionary<string, JArray>();
                Dictionary<string, List<string>> arrayParentWithChildForAllEntiry = new Dictionary<string, List<string>>();
                Dictionary<string, JObject> arrayJsonObjectForAllEntiry = new Dictionary<string, JObject>();


                var columnsList = table.Columns.Cast<DataColumn>();
                if (arrayParentWithChildForAllEntiry.Values.Count == 0)
                {
                    var arrayListWholeList = columnsList.Where(column => column.ToString().Contains("[") && column.ToString().Contains("]")).ToList();

                    foreach (var parentWithChild in arrayListWholeList)
                    {
                        var parentName = parentWithChild.ToString().Split('[').First();
                        var childName = parentWithChild.ToString().Split('[', ']')[1];
                        if (!arrayParentWithChildForAllEntiry.ContainsKey(parentName))
                        {
                            arrayParentWithChildForAllEntiry.Add(parentName, new List<string>() { childName });
                        }
                        else
                        {
                            arrayParentWithChildForAllEntiry[parentName].Add(childName);
                        }
                    }
                    foreach (var parent in arrayParentWithChildForAllEntiry)
                    {
                        var ch = new JObject();
                        foreach (var child in parent.Value.Distinct())
                        {
                            ch.Add(new JProperty(child, null));
                        }
                        arrayJsonObjectForAllEntiry.Add(parent.Key, ch);
                    }
                }

                string previous_id = string.Empty;
                StringBuilder bulkRequest = new StringBuilder();
                //int recordCount = 0;
                //foreach (var oneRow in wholeResultset)//.Where(element=>!(element.ToString().Contains('[') || element.ToString().Contains('.'))))
                for (int i = 0; i < wholeResultset.Count; )
                {
                    var oneRow = wholeResultset[i];

                    if (previous_id != oneRow[primaryKey].ToString())
                    {
                        var eachRecord = new JObject();
                        foreach (var colum in columnsList)
                        {
                            if (!colum.ToString().Contains("["))
                            {
                                if (colum.ToString().Contains("."))
                                {
                                    var parentName = colum.ToString().Split('.').First();
                                    var childName = colum.ToString().Split('.')[1];
                                    if (eachRecord.Children<JProperty>().Any(p => p.Name == parentName))
                                        eachRecord[parentName].Children<JProperty>().First()
                                            .AddAfterSelf(new JProperty(childName, oneRow[colum.ToString()].ToString().Trim() == "" ? null : oneRow[colum.ToString()].ToString().Trim()));
                                    else
                                        eachRecord
                                            .Add(new JProperty(parentName, new JObject(new JProperty(childName, oneRow[colum.ToString()].ToString().Trim() == "" ? null : oneRow[colum.ToString()].ToString().Trim()))));
                                }
                                else
                                    eachRecord
                                        .Add(new JProperty(colum.ToString(), oneRow[colum.ToString()].ToString().Trim() == "" ? null : oneRow[colum.ToString()].ToString().Trim()));
                            }
                        }

                        foreach (var child in wholeResultset.Where(element => element[primaryKey].ToString() == oneRow[primaryKey].ToString()).ToList())
                        {
                            foreach (var oneParent in arrayJsonObjectForAllEntiry)
                            {
                                var selectedColumn = columnsList.Where(column => column.ToString().StartsWith(oneParent.Key + "[")).ToList();
                                var arrayObj = new JObject();// oneParent.Value;
                                foreach (var col in selectedColumn)
                                {
                                    var childName = col.ToString().Split('[', ']')[1];
                                    arrayObj[childName] = child[col.ToString()].ToString().Trim() == "" ? null : child[col.ToString()].ToString().Trim();
                                }
                                if (!arrayJsonObjectValuesForAllEntiry.ContainsKey(oneParent.Key))
                                {
                                    arrayJsonObjectValuesForAllEntiry.Add(oneParent.Key, new JArray() { arrayObj });
                                }
                                else
                                {
                                    string acualArray = arrayJsonObjectValuesForAllEntiry[oneParent.Key].ToString().Replace(" ", "").Replace("\r\n", "");
                                    string compareWith = new JArray(arrayObj).ToString().Trim('[', ']').Replace(" ", "").Replace("\r\n", "");
                                    if (!acualArray.Contains(compareWith))
                                        arrayJsonObjectValuesForAllEntiry[oneParent.Key].Add(arrayObj);
                                }
                            }
                        }
                        foreach (var addArray in arrayJsonObjectValuesForAllEntiry)
                        {
                            if (addArray.Key.ToString().Contains('.'))
                            {
                                var parentName = addArray.Key.ToString().Split('.').First();
                                var childName = addArray.Key.ToString().Split('.', '[', ']')[1];
                                if (eachRecord[parentName] == null)
                                {
                                    eachRecord.Add(parentName, new JObject(new JProperty(childName, addArray.Value)));
                                }
                                else
                                {
                                    eachRecord[parentName].Children().First().AddAfterSelf(new JProperty(childName, addArray.Value));
                                }
                            }
                            else
                            {
                                eachRecord.Add(new JProperty(addArray.Key, addArray.Value));
                            }
                        }
                        finalResult.Add(new JObject(eachRecord));

                    }
                    previous_id = oneRow[primaryKey].ToString();
                    var noOfRowDeleted = wholeResultset.RemoveAll(element => element[primaryKey].ToString() == oneRow[primaryKey].ToString());
                }
            }
            return JsonConvert.SerializeObject(finalResult);
        }

    }
}
