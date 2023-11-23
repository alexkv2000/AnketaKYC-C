using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


using ExempleAnketaKYCService.DeserializeClasses;
using GAZ.AnketaKYC;
using FileInfo = System.IO.FileInfo;

/*using System.Text;
using System.Threading.Tasks;*/

namespace ExempleAnketaKYCService
{
    class Program
    {
        public Program()
        {
        }
        //static HttpClient client = new HttpClient();

        //public static url = @""

        static async Task Main(string[] args)
        {
            int maxParallelTask = 1;
            String sPathFiles = "c:/Downloads/34477/";
            long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(">>");
            //GetAnkets(); // Получение списка
            //GetAnketInfo(34477);// получение информации по анкете(XML)
            //Console.ReadKey();
            String[] sFiles = {"https://kyc-compliance.ru/upload/iblock/fc8/wkqkefum64oritimkmmkzgl40z2dpn74/Анкета-НАК.pdf",
            "https://kyc-compliance.ru/xls_files/ques_34477.xls",
            "https://kyc-compliance.ru/upload/iblock/c76/geoymcnr6wfhaoetmmfbwhesik22g821/Скан копия устава дляt ООО dСАФ-ХОЛЛАНД Русq.pdf",
            "https://kyc-compliance.ru/upload/iblock/6c4/x570ulje3oaw7vds5nag5d14dw9jk6ci/RUS_Minutes_extraordinary meeting 2022.11.03_signed.PDF",
            "https://kyc-compliance.ru/upload/iblock/0ba/r002fdgqzzkeaxw3kfjd2p3qlapy8tv8/egrul.pdf",
            "https://kyc-compliance.ru/upload/iblock/9ce/m6vu7h1ax0i1u0moyd8ykdseh88fdloy/beneficiar.pdf"};
             
             await DownloadFiles(maxParallelTask,sFiles,sPathFiles);
             
             var stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
             Console.WriteLine("<<");
             //Console.ReadKey();
             Console.WriteLine($"время выполнения {(DateTimeOffset.FromUnixTimeMilliseconds(stop) - DateTimeOffset.FromUnixTimeMilliseconds(start))} секунд");
            // Console.WriteLine((dateTimeOffsetStart).ToString("yyyy-MM-dd HH:mm:ss.fff"));
           //  Console.WriteLine((dateTimeOffsetStop).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public static async Task DownloadFiles(int maxParallelDownload,string[] urls,string filePaths)
        {
            Console.WriteLine(maxParallelDownload);
            // Проверяем доступность файла по указанному пути
            FileDownloader downloader = new FileDownloader(maxParallelDownloads: maxParallelDownload); // Ограничить поток
            await downloader.DownloadFilesWithLimitedParallelism(urls, filePaths);
        }

        public class FileDownloader
        {
            private SemaphoreSlim semaphore;

            public FileDownloader(int maxParallelDownloads)
            {
                semaphore = new SemaphoreSlim(maxParallelDownloads);
            }

            public async Task DownloadFilesWithLimitedParallelism(string[] urls, string pathFile)
            {
                Boolean statisticProgress = false;
                Boolean statisticFileCompleted = true;
                
                Task[] downloadTasks = new Task[urls.Length];
        
                for (int i = 0; i < urls.Length; i++)
                {
                    string url = urls[i];
                    string filePath = pathFile + Path.GetFileName(urls[i]);
            
                    downloadTasks[i] = Task.Run
                    (
                        async () =>
                    {
                        await semaphore.WaitAsync(); // Ожидаем доступ к семафору
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                if (statisticProgress)
                                {
                                    client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                }
                                if (statisticFileCompleted)
                                {
                                    client.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                                }
                                // Проверяем доступность файла по указанному пути
                                // Получаем размер уже существующего файла, чтобы возобновить закачку с этой позиции
                                long existingFileSize = new FileInfo(filePath).Length;
                                //проверяем размер url файла
                                //client.OpenRead(url);
                                //long size = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                                //Console.Write($"Размер файла url: {size} байт ");
                                //Console.WriteLine($"- Files: {existingFileSize} байт");
                                    // Добавляем заголовок Range в запросе для возобновления закачки с указанной позиции
                                if (existingFileSize>0) {client.Headers.Add("Content-Range", $"bytes {existingFileSize}-");}
                                await client.DownloadFileTaskAsync(url, filePath);
                                // Обработка событий, например, для отслеживания прогресса закачки

                            }
                        }
                        finally
                        {
                            semaphore.Release(); // Освобождаем семафор после выполнения задачи
                        }
                    });
                }
        
                await Task.WhenAll(downloadTasks);
            }

            private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    Console.WriteLine("Закачка завершена успешно");
                }
                else
                {
                    Console.WriteLine("Ошибка при закачке: " + e.Error.Message);
                }
            }

            private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                Console.WriteLine("Закачано {0} байт из общего размера {1} байт", e.BytesReceived, e.TotalBytesToReceive);

            }
        }
        public class PerformersGroupInfo
        {
            public Guid LEID;
            public Guid GroupID;
        }

        public class ExternalTagsWorker
        {
            private Dictionary<string, string> TagsInfo { get; set; }

            public ExternalTagsWorker(string docInfo)
            {
                if (string.IsNullOrWhiteSpace(docInfo)) { TagsInfo = new Dictionary<string, string>(); return; }
                try
                {
                    TagsInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(docInfo);
                }
                catch { TagsInfo = new Dictionary<string, string>(); }
            }

            public ExternalTagsWorker()
            {
                TagsInfo = new Dictionary<string, string>();
            }

            public bool IsNull()
            {
                return TagsInfo == null || (TagsInfo != null && !TagsInfo.Any());
            }

            public string this[string tag]
            {
                get
                {
                    if (TagsInfo.ContainsKey(tag)) { return TagsInfo[tag]; }
                    else
                    {
                        TagsInfo.Add(tag, "");
                        return TagsInfo[tag];
                    }
                }
                set
                {
                    if (TagsInfo.ContainsKey(tag)) { TagsInfo[tag] = value; }
                    else { TagsInfo.Add(tag, value); }
                }
            }

            public void RemoveTag(string tag)
            {
                if (TagsInfo.ContainsKey(tag)) { TagsInfo.Remove(tag); }
            }

            public bool TryParseTagValue<T>(string tag, out T value)
            {
                if (TagsInfo.ContainsKey(tag))
                {
                    try { value = JsonConvert.DeserializeObject<T>(TagsInfo[tag]); return true; } catch { value = default(T); return false; }
                }
                else { value = default(T); return false; }
            }

            public string GetStringData()
            {
                return JsonConvert.SerializeObject(TagsInfo);
            }
        }

        public static void AddEDIApprovers(ApprovalStage approvalStage, out List<Guid> approvalList, Dictionary<Stage, Func<Guid, List<Guid>>> customProcessing = null)
        {
            approvalList = null;
            if (approvalStage.ID == RegistrationStageID)
            {
                Console.WriteLine($"approvalList is null? = {approvalList == null}");

                if (customProcessing != null && customProcessing.ContainsKey(Stage.Registration))
                {
                    Console.WriteLine($"customProcessing != null");
                    approvalList = customProcessing[Stage.Registration](approvalStage.ID);
                    if (approvalList != null)
                    {
                        Console.WriteLine($"approvalList.Count = {approvalList.Count} approvalList.FirstOrDefault ={approvalList.FirstOrDefault()}");
                    }
                    else Console.WriteLine($"approvalList = null");
                }
                else
                {
                    Console.WriteLine($"customProcessing != null");
                    Guid registrar = new Guid("2A58E486-94C1-4943-AABD-F80BE39EE29B");
                    if (registrar != Guid.Empty) { approvalList = new List<Guid> { registrar }; }
                }
            }
            else {
                Console.WriteLine($"approvalStage.ID = {approvalStage.ID}");
            }
        }

        internal class ApprovalStage
        {
            public Guid ID;
        }

        public static readonly Guid RegistrationStageID = new Guid("{89285803-7ad6-4bbb-a4b3-5ede788f1c8a}");

        public enum Stage
        {
            Registration,
            AdditionalInfoStage,
            Signing,
            PartnerSigning,
            ExecutionStage
        }


        /// <summary>
        /// Получение xml файла анкеты (пока в тестовом формате) по id анкеты
        /// </summary>
        /// <returns></returns>
        async public static void /*ChangeStatusEnum*/ CahngeStatus(int id, string status)
        {            
            string res;

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://kyc-compliance.ru/api/set_status.php?ID={id}&STAT={status}";
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
               // HttpResponseMessage Response = client.GetAsync(url).GetAwaiter().GetResult();
                HttpResponseMessage Response = await client.GetAsync(url);
                Console.WriteLine("ReasonPhrase: " + Response.ReasonPhrase);
                Console.WriteLine("Content: " + Response.RequestMessage.Content);
               // res = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                res = await Response.Content.ReadAsStringAsync();
            }
            Console.WriteLine(res);
            ChangeStatusResponse.ChangeStatusEnum? response = null;
            using (TextReader reader = new StringReader(res))
            {
                response = (ChangeStatusResponse.ChangeStatusEnum)new XmlSerializer(typeof(ChangeStatusResponse.ChangeStatusEnum)).Deserialize(reader);
            }

            Console.WriteLine("response: " + response);
            // return response.Result;

        }

        /// <summary>
        /// Получение списка id анкет готовых для выгрузки
        /// </summary>
        /// <returns></returns>
        async public static void GetAnkets()
        {
            string rez = null;

            using (HttpClient client = new HttpClient())
            {
                string URL = "https://kyc-compliance.ru/api/get_questionnaires.php";//Получение списка id анкет готовых для выгрузки

                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;

                HttpResponseMessage Response = await client.GetAsync(URL);
                Console.WriteLine("ReasonPhrase: " + Response.ReasonPhrase);
                Console.WriteLine("Content: " + Response.RequestMessage.Content);

                rez = await Response.Content.ReadAsStringAsync();
            }
            Console.WriteLine("*******************");
            Console.WriteLine(rez);
            Console.WriteLine("*******************");
            DownloadQuestionnaires AnketKYCs = null;

            using (TextReader reader = new StringReader(rez))
            {
                AnketKYCs = (DownloadQuestionnaires)new XmlSerializer(typeof(DownloadQuestionnaires)).Deserialize(reader);
                Console.WriteLine($"Получено карточек от сервиса Анкета KYC новых: {AnketKYCs.NEW.IDs.Count} на обновление: {AnketKYCs.RETURN.IDs.Count}");
                if (AnketKYCs.NEW.IDs.Count > 0)
                {
                    Console.WriteLine("NEW");
                    foreach (var VARIABLE in AnketKYCs.NEW.IDs)
                    {
                        Console.WriteLine(VARIABLE);
                    }
                }
                if (AnketKYCs.RETURN.IDs.Count > 0)
                {
                    Console.WriteLine("RETURN");
                    foreach (var VARIABLE in AnketKYCs.RETURN.IDs)
                    {
                        Console.WriteLine(VARIABLE);
                    }
                }
            }
        }

        /// <summary>
        /// Получение xml файла анкеты (пока в тестовом формате) по id анкеты
        /// </summary>
        /// <returns></returns>
        async public static void GetAnketInfo(int id) //был тип string
        {
            string rez = null;
            string mess = "";
            string URL = $"https://kyc-compliance.ru/api/download_questionnaires.php?ID={id}"; //Получение списка id анкет готовых для выгрузки
            Console.WriteLine(URL);

            using (HttpClient client = new HttpClient())
            {
                //client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                //HttpResponseMessage Response = client.GetAsync(URL).GetAwaiter().GetResult(); //заменил на : =>
                HttpResponseMessage Response = await client.GetAsync(URL);
                // rez = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); //заменил на : => 
                rez = await Response.Content.ReadAsStringAsync();
                Response.Dispose();
            }
            Console.WriteLine(rez);

            QuestionnaireInfo AnketKYCInfo = null;
            using (TextReader reader = new StringReader(rez))
            {
                AnketKYCInfo = (QuestionnaireInfo)new XmlSerializer(typeof(QuestionnaireInfo)).Deserialize(reader);
            }

            // Console.WriteLine($"INN.Values: {AnketKYCInfo.ContragentInfo.ContragentInfoValues.values.FirstOrDefault().INN.Values==null}");
            Console.WriteLine($"ContragentInfo INN: {AnketKYCInfo.ContragentInfo.Values.FirstOrDefault().INN.Values.FirstOrDefault()}");
            Console.WriteLine($"ContragentInfo NAME: {AnketKYCInfo.ContragentInfo.Values.FirstOrDefault().NAME?.Values.FirstOrDefault()}");
            Console.WriteLine($"KurEmail: {(AnketKYCInfo.KurEmail != null ? AnketKYCInfo.KurEmail.tagValues.values.FirstOrDefault() : "Null")}");
            Console.WriteLine($"GGAZLEInfo Name: {AnketKYCInfo.GGAZLEInfo.Values.FirstOrDefault().Name}");
            Console.WriteLine($"GGAZLEInfoSynctag Synctag: {AnketKYCInfo.GGAZLEInfo.Values.FirstOrDefault().Synctag}");
            Console.WriteLine($"SanctionRisk: {AnketKYCInfo.SanctionRisk?.Values?.FirstOrDefault() ?? "null"}");
            Console.WriteLine($"DueDiligence: {AnketKYCInfo.DueDiligence?.Values?.FirstOrDefault() ?? "null"}");
            Console.WriteLine($"TypeContract: {AnketKYCInfo.TypeContract?.Values?.FirstOrDefault() ?? "null"}");
            //Console.WriteLine($"Файл: {AnketKYCInfo.ContragentInfo?.Values?.FirstOrDefault()?.FILE?.FileInfo?.FileName ?? "null"}");
            // Console.WriteLine($"Ссылка на файл: {AnketKYCInfo.ContragentInfo?.Values?.FirstOrDefault()?.FILE?.FileInfo?.LinkFile ?? "null"}");
            Console.WriteLine($"ServiceStatus: {AnketKYCInfo.ServiceStatus}");

            List<string> mainFileLink = new List<string>();
            //Добавление файлов заполненой анкеты
            if (AnketKYCInfo.AnketFiles != null && AnketKYCInfo.AnketFiles.Values.Count > 0)
            {
                Console.WriteLine($"AnketFiles {AnketKYCInfo.AnketFiles.Values.Count}");
                string linkf = string.Empty;
                string namef = string.Empty;
                foreach (AnketFILE anketFILE in AnketKYCInfo.AnketFiles.Values)
                {
                    linkf = anketFILE.FileInfo.LinkFile ?? string.Empty;
                    namef = anketFILE.FileInfo.FileName ?? string.Empty;
                    if (!linkf.IsEmpty() && !namef.IsEmpty())
                    {
                        Console.WriteLine($"Файл: {namef ?? "null"}");
                        Console.WriteLine($"Ссылка на файл: {linkf ?? "null"}");
                        //AddFileInCard(doc, DocumentFileType.Main, ServicrKYC_CardID, linkf, namef);
                        mainFileLink.Add(linkf);
                    }
                }
            }

            //Добавление файлов заполненой анкеты Exele
            if (AnketKYCInfo.AnketFilesXLS != null && AnketKYCInfo.AnketFilesXLS.Values.Count > 0)
            {
                Console.WriteLine($"AnketFilesXLS {AnketKYCInfo.AnketFilesXLS.Values.Count}");
                string linkf = string.Empty;
                string namef = string.Empty;
                foreach (AnketFILE anketFILExls in AnketKYCInfo.AnketFilesXLS.Values)
                {
                    linkf = anketFILExls.FileInfo.LinkFile ?? string.Empty;
                    namef = anketFILExls.FileInfo.FileName ?? string.Empty;
                    if (!linkf.IsEmpty() && !namef.IsEmpty())
                    {
                        Console.WriteLine($"Файл: {namef ?? "null"}");
                        Console.WriteLine($"Ссылка на файл: {linkf ?? "null"}");
                        //AddFileInCard(doc, DocumentFileType.Main, ServicrKYC_CardID, linkf, namef);
                        mainFileLink.Add(linkf);
                    }
                }
            }

            Console.WriteLine($"Остальные файлы:");
            List<DeserializeClasses.FileInfo> allfiles = GetAllFiles(rez, mainFileLink);
            allfiles.ForEach(x => Console.WriteLine($"FileName = {x.FileName} LinkFile = {x.LinkFile}"));


          //  return null;
           
        }

        

        internal static List<DeserializeClasses.FileInfo> GetAllFiles(string xmlAnket, List<string> mainFiles)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xmlAnket);
            System.Xml.XmlNodeList xmlNodeList = doc.GetElementsByTagName("document");//Получаем все теги с названием "document"

            List<DeserializeClasses.FileInfo> allfiles = new List<DeserializeClasses.FileInfo>();
            foreach (System.Xml.XmlElement node in xmlNodeList)
            {
                if (mainFiles != null && mainFiles.Count > 0 && mainFiles.Any(x => x.Equals(node?.Attributes["xlink"].Value, StringComparison.InvariantCultureIgnoreCase))) continue;
                allfiles.Add(new DeserializeClasses.FileInfo() { LinkFile = node?.Attributes["xlink"].Value, FileName = node?.InnerText });
            }
            doc = null;
            return allfiles.DistinctBy(x => x.LinkFile).ToList();//Исключаем повторяющиеся по параметру LinkFile
        }

        async public static void GetFile(string URL, string FilePath, string FileName)
        {
            Console.WriteLine("1 Start GetFile URL:" + URL);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                Console.WriteLine("2 Start Download");
               // using (HttpResponseMessage response = client.GetAsync(URL).GetAwaiter().GetResult())
                using (HttpResponseMessage response = await client.GetAsync(URL))
                {
                    //File.WriteAllBytes(FilePath, response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult());
                    Console.WriteLine("StatusCode: " + response.StatusCode.ToString()); 
                   //using (Stream streamToReadFrom = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()) заменил на =>
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        //File.WriteAllBytes(FilePath, streamToReadFrom.));
                        using (FileStream fstream = new FileStream(FilePath, FileMode.OpenOrCreate))
                        {
                            Console.WriteLine("fstream.Name: " + fstream.Name);
                            Console.ReadKey();
                            streamToReadFrom.CopyTo(fstream);
                            
                        }
                    }
                }
            }

            Console.WriteLine("GetFile end");
        }

        /// <summary>
        /// Авторизация в сервисе KYC
        /// </summary>
        public static class AuthorizationKYC
        {
            /// <summary>
            /// Логин для сервиса KYC
            /// </summary>
            private static string username = "api";
            /// <summary>
            /// Пароль для сервиса KYC
            /// </summary>
            private static string pwd = "szuWm7^8S184S05%FNy!";
            /// <summary>
            /// Авторизация для сервиса KYC
            /// </summary>
            public static readonly AuthenticationHeaderValue AuthorKYC = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{username}:{pwd}")));
        }


    }

    public static class AnketKYCServiceHelper
    {
        /// <summary>
        /// ИД шаблона БП по передаче статуса сервису Анкета KYC
        /// </summary>
        public static Guid SendStatusProcessID = new Guid("8BBF9589-EBBD-4E3A-B194-3718D4586DF4");

        public class BPChangeStatusVarible
        {
            public int? ID { get; set; }
            public string Status { get; set; }
        }

        public enum AnketServiceStatusT
        {
            /// <summary>
            /// Редактирование
            /// </summary>
            edit,
            /// <summary>
            /// На доработке
            /// </summary>
            return_edit,
            /// <summary>
            /// Отменена
            /// </summary>
            cancel,
            /// <summary>
            /// Отправлена
            /// </summary>
            sent
        }

        public static string SerializeToJson(this BPChangeStatusVarible varible)
        {
            if (varible == null) return null;
            return JsonConvert.SerializeObject(varible);
        }

        public static BPChangeStatusVarible DeserializeJson(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            try
            {
                return JsonConvert.DeserializeObject<BPChangeStatusVarible>(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось преобразовать строковое значение в класс BPChangeStatusVarible. Ошибка:" + Environment.NewLine + ex.ToString());
            }
            return null;
        }
    }

    public static class Compare
    {
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(Compare.By(identitySelector));
        }

        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(identitySelector(x), identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return identitySelector(obj).GetHashCode();
            }
        }

        /// <summary>
		/// Проверка строки на пустое значение
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }



}
