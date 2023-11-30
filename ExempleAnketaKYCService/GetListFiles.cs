using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GAZ.AnketaKYC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExempleAnketaKYCService
{
    public class GetListFiles
    {
        public static async Task Main(string[] args)
        {
            string rez = null;
            string URL =
                $"https://kyc-compliance.ru/api/download_questionnaires.php?ID={args[0]}"; //Получение списка id анкет готовых для выгрузки
            //Console.WriteLine($"Ссылка на анкету : {URL}");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Authorization = Program.AuthorizationKYC.AuthorKYC;
                rez = await client.GetStringAsync(URL).ConfigureAwait(false);
            }

            //Console.WriteLine(rez);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(rez);

            // Получаем все элементы с атрибутом xlink
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("//document[@xlink]");
            // Получаем значения выбранных атрибутов и выводим их
            List<string> Attribute = null;
            foreach (XmlNode node in nodes)
            {
                string selectedAttribute = node.Attributes["xlink"].Value;
                Console.WriteLine(selectedAttribute);
                // Attribute.Add(selectedAttribute);
            }
            // return Attribute;
        }
    }
}


public class AuthorizationKYC
{
    public static string username = "api";
    public static string pwd = "szuWm7^8S184S05%FNy!";

    public static readonly AuthenticationHeaderValue AuthorKYC = new AuthenticationHeaderValue("Basic",
        Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{username}:{pwd}")));

    public AuthenticationHeaderValue AuthorKyc { get; }
}