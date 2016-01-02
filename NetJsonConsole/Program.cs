using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Data;
using System.IO;
using Newtonsoft.Json;


namespace NetJsonConsole
{
    class Program
    {
        /// <summary>
        /// Simple class demonstrating how to convert data into JSON and back again
        /// Ex 1 -> Programmer-defined object model, like something you would find a data framework, to JSON
        /// Ex 2 -> DataTable to JSON: using Json.NET
        ///     Nuget package Newtonsoft.Json was used for high performance conversions
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // instantiate some data
            ObjectModel[] dataArray = new ObjectModel[3];
            for (int i = 0; i < 3; i++)
            {
                dataArray[i] = new ObjectModel() 
                {
                    ID = i,
                    Value = "Object " + i,
                    Deleted = false
                };
            }

            // serialize the data
            string json = getString(dataArray);
            Console.WriteLine(json);

            // Now do it with a datatable
            DataTable dt = new DataTable("MyRows");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Deleted", typeof(bool));
            for (int i = 0; i < 3; i++)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = i;
                dr["Value"] = "Row " + i;
                dr["Deleted"] = false;
                dt.Rows.Add(dr);
            }

            json = getStringFromDataTable(dt);
            Console.WriteLine(json);

            // wait for the enter key before finishing
            Console.ReadLine();
        }

        // gets the JSON String of data from the data array
        private static string getString(ObjectModel[] dataArray)
        {
            string formattedJson = string.Empty;

            try
            {

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ObjectModel[]));
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSerializer.WriteObject(ms, dataArray);
                    if (ms != null)
                    {
                        ms.Position = 0;
                        StreamReader sr = new StreamReader(ms);
                        formattedJson = sr.ReadToEnd();
                    }    
                }
            }
            catch (InvalidDataContractException e1)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Invalid Data Contract found when serializing"));
            }
            catch (SerializationException e2)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Serialization Exception has occurred"));
            }
            catch (System.ServiceModel.QuotaExceededException e3)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("The message quoata has been exceeded"));
            }

            return formattedJson;
        }

        // gets the JSON Strring of data from the DataTable
        private static string getStringFromDataTable(DataTable dataTable)
        {
            string formattedJson = string.Empty;

            try
            {
                string table = JsonConvert.SerializeObject(dataTable);
                formattedJson = table;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Hit Exception in JsonConvert.SerializeObject");
            }

            return formattedJson;
        }
    }
}
