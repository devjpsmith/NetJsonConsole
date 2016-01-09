using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Data;
using System.IO;
using System.IO.Compression;
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
            testObjectModel();

            // Now do it with a datatable
            // let's use a larger dataset to test performance
            //testDateTable();

            // wait for the enter key before finishing
            Console.ReadLine();
        }

        private static void testObjectModel()
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
            byte[] json = getJsonBytes(dataArray);

            List<ObjectModel> objectModelList = getObjectModelList(json);

        }

        private static List<ObjectModel> getObjectModelList(byte[] jsonData)
        {
            string json = string.Empty;
            using (MemoryStream ms = new MemoryStream(jsonData))
            {
                int nRead = 0;
                byte[] buffer = new byte[1024];
                while ((nRead = ms.Read(buffer, 0, buffer.Length)) == 1024)
                {
                    json += Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                }
                json += Encoding.UTF8.GetString(buffer, 0, nRead);
            }

            return JsonConvert.DeserializeObject<List<ObjectModel>>(json);
        }

        // demonstrate using DataTables
        private static void testDateTable()
        {
            DataTable dt = new DataTable("MyRows");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Year", typeof(int));
            dt.Columns.Add("SerialNumber", typeof(string));
            dt.Columns.Add("Cost", typeof(string));
            dt.Columns.Add("MSRP", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(DateTime));
            dt.Columns.Add("LastModifiedDate", typeof(DateTime));
            dt.Columns.Add("Deleted", typeof(bool));

            DateTime now = DateTime.UtcNow;

            for (int i = 0; i < 5000; i++)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = i;
                dr["Value"] = "Row " + i;
                dr["Description"] = "Car";
                dr["Make"] = "MyMake";
                dr["Model"] = "MyModel";
                dr["Year"] = 2016;
                dr["SerialNumber"] = "DF78E8C" + i;
                dr["Cost"] = DBNull.Value;
                dr["MSRP"] = "$30,000";
                dr["CreatedDate"] = now;
                dr["LastModifiedDate"] = DateTime.UtcNow;
                dr["Deleted"] = false;
                dt.Rows.Add(dr);
            }

            now = DateTime.Now;
            string jsonStr = getStringFromDataTable(dt);
            string path = "C:/TestJson.txt";

            DateTime complete = new DateTime();

            byte[] bytes = getCompressedData(jsonStr);
            complete = DateTime.Now;

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                // write the output to a file   
                fs.Write(bytes, 0, bytes.Length);
            }

            Console.WriteLine(string.Format("complete in: {0:ss} sec", (complete - now).TotalSeconds));


            // let's decompress and read this now
            jsonStr = getDecompressedDate(bytes);

            path = "C:/TestJsonOut.txt";
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            File.WriteAllText(path, jsonStr, Encoding.UTF8);

        }

        // gets the JSON String of data from the data array
        private static byte[] getJsonBytes(ObjectModel[] dataArray)
        {         
            string jsonStr = JsonConvert.SerializeObject(dataArray);

            return Encoding.UTF8.GetBytes(jsonStr);

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

        private static byte[] getCompressedData(string data)
        {
            // Encode the string into a UTF-8 byte array
            byte[] uncompressed = Encoding.UTF8.GetBytes(data);
            byte[] compressed = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    // using GZIP, compress the underlying stream
                    zipStream.Write(uncompressed, 0, uncompressed.Length);
                }
                // assign the stream array to our return value
                compressed = ms.ToArray();
            }
            // return the compressed array
            return compressed;
        }

        private static string getDecompressedDate(byte[] data)
        {
            string json = string.Empty;
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[1024];
                    int nRead;
                    while ((nRead = zipStream.Read(buffer, 0, buffer.Length)) == 1024)
                    {
                        json += Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    }
                    json += Encoding.UTF8.GetString(buffer, 0, nRead);
                } 
            }
               
            return json;
        }
    }
}
