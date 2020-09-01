using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NotissimusAngleSharp
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            //Url from where we get our XML
            string url = "http://partner.market.yandex.ru/pages/help/YML.xml";
            //Name of the objects to find
            string objectName = "offer";
            //Attribute of the objects to write to file
            string attributeName = "id";
            //Path to file
            string path = "id.txt";

            string source = await GetContentAsync(url);

            var list = await GetObjectsFromXmlAsync(source, objectName);


            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (IElement i in list)
                    {
                        sw.WriteLine(i.GetAttribute(attributeName));
                    }
                }
            }

        }

        /// <summary>
        /// Sends GET request to specified URL and returns response content.
        /// </summary>
        /// <param name="url">Url to send request to.</param>
        /// <returns>Url response content</returns>
        public static async Task<string> GetContentAsync(string url)
        {
            //Get our xml from server
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //Log error
                    Console.WriteLine("Error. Server sent: " + response.StatusCode);
                    return null;
                }

            }
        }

        /// <summary>
        /// Returns specified objects from specified XML source.
        /// </summary>
        /// <param name="source">XML to get objects from.</param>
        /// <param name="objectName">Name of objects to return.</param>
        /// <returns>IEnumerable array of objects.</returns>
        public static async Task<IEnumerable> GetObjectsFromXmlAsync(string source, string objectName)
        {
            //Use the default configuration for AngleSharp
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Cpecify the document to load
            var document = await context.OpenAsync(req => req.Content(source));

            //Linq query document to get objects with specified name
            var linq = document.All.Where(o => o.LocalName == objectName.ToLower());

            return linq;
        }




    }
}
