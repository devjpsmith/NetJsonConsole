using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;


namespace NetJsonConsole
{
    class Program
    {
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
    }
}
