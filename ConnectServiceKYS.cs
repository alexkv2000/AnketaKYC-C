
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GAZ.AnketaKYC.DeserializeClasses;
using FileInfo = System.IO.FileInfo;

namespace GAZ.AnketaKYC
{
    class ConnectServiceKYS
    {
        /// <summary>
        /// Общая ссылка на api
        /// </summary>
        //private static string ApiURL = "https://gaz.t2.ipg4you.com/api/";

        /// <summary>
        /// Ссылка на api(метод) для получения списка Анкет KYC готовых к выгрузке
        /// </summary>
        private static string UrlQuestionnairesList = "https://gaz.t2.ipg4you.com/api/get_questionnaires.php";//Получение списка id анкет готовых для выгрузки
        /// <summary>
        /// Ссылка на api(метод) для получения xml Анкеты KYC
        /// </summary>
        private static string GetInfoURL = "https://gaz.t2.ipg4you.com/api/download_questionnaires.php?ID=";//Получение xml анкеты по id
        /// <summary>
        /// Ссылка на api(метод) для смены статуса Анкеты KYC
        /// </summary>
        private static string ChangeStatusSURL = "https://gaz.t2.ipg4you.com/api/set_status.php?ID={ID}&STAT={STATUS}";//Получение xml анкеты по id

        internal static QuestionnairesList GetAnkets(string ApiURL)
        {
            string rez = null;

            using (HttpClient client = new HttpClient())
            {
                string url = $"{ApiURL}get_questionnaires.php";
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                HttpResponseMessage Response = client.GetAsync(url).GetAwaiter().GetResult();
                rez = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            QuestionnairesList AnketKYCs = null;

            using (TextReader reader = new StringReader(rez))
            {
                AnketKYCs = (QuestionnairesList)new XmlSerializer(typeof(QuestionnairesList)).Deserialize(reader);
            }
            return AnketKYCs;
        }

        /// <summary>
        /// Получение xml файла анкеты (пока в тестовом формате) по id анкеты
        /// </summary>
        /// <returns></returns>
        internal static QuestionnaireInfo GetAnketInfo(string ApiURL, int id, out string error)
        {
            string rez = null;
            error = null;

            using (HttpClient client = new HttpClient())
            {
                string url = $"{ApiURL}download_questionnaires.php?ID={id}";
                //client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                HttpResponseMessage Response = client.GetAsync(url).GetAwaiter().GetResult();
                rez = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            QuestionnaireInfo AnketKYCInfo = null;
            using (TextReader reader = new StringReader(rez))
            {
                try
                {
                    AnketKYCInfo = (QuestionnaireInfo)new XmlSerializer(typeof(QuestionnaireInfo)).Deserialize(reader);
                }
                catch (Exception ex)
                {
                    error =  $"Не удалось спарсит XML анкеты.{Environment.NewLine} Ошибка: {ex.StackTrace}";
                    return null;
                }
                AnketKYCInfo.TextXml = rez;
                AnketKYCInfo.AnketID = id;
            }

            return AnketKYCInfo;

        }

        internal static void DownloadFile(string URL, string FilePath)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                using (HttpResponseMessage response = client.GetAsync(URL).GetAwaiter().GetResult())
                {
                    using (Stream streamToReadFrom = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        using (FileStream fstream = new FileStream(FilePath, FileMode.OpenOrCreate))
                        {
                            streamToReadFrom.CopyTo(fstream);
                        }
                    }
                }
            }
        }

    //     public class DownloadFilesKYC
    // {
        //TODO static async Task Main(string[] args)
        // async Task Main(string[] args)
        // {
        //     int maxParallelTask = 4;
        //     String sPathFiles = "c:/Downloads/34477/";
        //     long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //     Console.WriteLine($">> {start}");
        //
        //     String[] arrayFiles =
        //     {
        //         "https://kyc-compliance.ru/upload/iblock/fc8/wkqkefum64oritimkmmkzgl40z2dpn74/Анкета-НАК.pdf",
        //         "https://kyc-compliance.ru/xls_files/ques_34477.xls",
        //         "https://kyc-compliance.ru/upload/iblock/c76/geoymcnr6wfhaoetmmfbwhesik22g821/Скан копия устава дляt ООО dСАФ-ХОЛЛАНД Русq.pdf",
        //         "https://kyc-compliance.ru/upload/iblock/6c4/x570ulje3oaw7vds5nag5d14dw9jk6ci/RUS_Minutes_extraordinary meeting 2022.11.03_signed.PDF",
        //         "https://kyc-compliance.ru/upload/iblock/0ba/r002fdgqzzkeaxw3kfjd2p3qlapy8tv8/egrul.pdf",
        //         "https://kyc-compliance.ru/upload/iblock/9ce/m6vu7h1ax0i1u0moyd8ykdseh88fdloy/beneficiar.pdf"
        //     };
        //
        //     Console.WriteLine($"Запущено {maxParallelTask} поток(а/ов):");
        //     await DownloadFiles(maxParallelTask, arrayFiles, sPathFiles, false, true);
        //
        //     var stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //     Console.WriteLine("<<");
        //
        //     //Console.ReadKey();
        //     Console.WriteLine(
        //         $"время выполнения {(DateTimeOffset.FromUnixTimeMilliseconds(stop) - DateTimeOffset.FromUnixTimeMilliseconds(start))} секунд");
        //     // Console.WriteLine((dateTimeOffsetStart).ToString("yyyy-MM-dd HH:mm:ss.fff")); Console.WriteLine((dateTimeOffsetStop).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        // }

        // public static async Task DownloadFiles(int maxParallelDownload, string[] urls, string filePaths, Boolean statProgress, Boolean statFileCompleted)
        public static async Task DownloadFiles(int maxParallelDownload, string[] urls, string filePaths)
        {
            FileDownloader
                downloader = new FileDownloader(maxParallelDownloads: maxParallelDownload); // Ограничить поток
            await downloader.DownloadFilesWithLimitedParallelism(urls, filePaths/*, statProgress, statFileCompleted*/);
        }
        protected class FileDownloader
        {
            private SemaphoreSlim semaphore;
            public FileDownloader(int maxParallelDownloads)
            {
                semaphore = new SemaphoreSlim(maxParallelDownloads);
            }
            protected internal async Task DownloadFilesWithLimitedParallelism(string[] urls, string pathFile/*, Boolean statProgress, Boolean statFileCompleted*/)
            {
                // Boolean statisticProgress = statProgress;
                // Boolean statisticFileCompleted = statFileCompleted;

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
                                    // if (statisticProgress)
                                    // {
                                    //     client.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                    // }
                                    // if (statisticFileCompleted)
                                    // {
                                    //     client.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                                    // }
                                    // Проверяем доступность файла по указанному пути
                                    // Получаем размер уже существующего файла, чтобы возобновить закачку с этой позиции
                                    // long existingFileSize = 0;
                                    // try
                                    // {
                                    //     existingFileSize = new FileInfo(filePath).Length;
                                    // }
                                    // catch (Exception e)
                                    // {
                                    //     Console.WriteLine($"Сообщение: {e.Message}");
                                    // }
                                    // Добавляем заголовок Range в запросе для возобновления закачки с указанной позиции
                                    //client.Headers.Add("Content-Range", $"bytes {existingFileSize}-");
                                    await client.DownloadFileTaskAsync(url, filePath);
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
            // private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            // {
            //     if (e.Error == null)
            //     {
            //         Console.WriteLine("Закачка завершена успешно");
            //     }
            //     else
            //     {
            //         Console.WriteLine("Ошибка при закачке: " + e.Error.Message);
            //     }
            // }
            // private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            // {
            //     Console.WriteLine("Закачано {0} байт из общего размера {1} байт", e.BytesReceived,
            //         e.TotalBytesToReceive);
            // }
        }
    // }
        /// <summary>
        /// Передача статуса на сервис АнкетаKYC
        /// </summary>
        /// <returns></returns>
        internal static ChangeStatusResponse.ChangeStatusEnum? CahngeStatus(string ApiURL, int id, string Status)
        {
            string res;

            using (HttpClient client = new HttpClient())
            {
                string url = $"{ApiURL}set_status.php?ID={id}&STAT={Status}";
                client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                HttpResponseMessage Response = client.GetAsync(url).GetAwaiter().GetResult();
                res = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            ChangeStatusResponse.ChangeStatusEnum? response = null;
            using (TextReader reader = new StringReader(res))
            {
                response = (ChangeStatusResponse.ChangeStatusEnum)new XmlSerializer(typeof(ChangeStatusResponse.ChangeStatusEnum)).Deserialize(reader);
            }

            return response;

        }

       /* public static Exception GetRequest(string url, out RequestResult<T> error)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = AuthorizationKYC.AuthorKYC;
                    HttpResponseMessage Response = client.GetAsync(url).GetAwaiter().GetResult();
                    Response.StatusCode
                    res = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {

            }
        }*/


    }

    

    /// <summary>
    /// Авторизация в сервисе KYC
    /// </summary>
    internal static class AuthorizationKYC
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
