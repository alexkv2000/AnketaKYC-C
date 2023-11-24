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
using ExempleAnketaKYCService.DeserializeClasses;
using GAZ.AnketaKYC;
using FileInfo = System.IO.FileInfo;

namespace ExempleAnketaKYCService
{
    public class DownloadFilesKYC
    {
        static async Task Main(string[] args)
        {
            int maxParallelTask = 2;
            String sPathFiles = "c:/Downloads/34477/";
            long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine($">> {start}");

            String[] sFiles =
            {
                "https://kyc-compliance.ru/upload/iblock/fc8/wkqkefum64oritimkmmkzgl40z2dpn74/Анкета-НАК.pdf",
                "https://kyc-compliance.ru/xls_files/ques_34477.xls",
                "https://kyc-compliance.ru/upload/iblock/c76/geoymcnr6wfhaoetmmfbwhesik22g821/Скан копия устава дляt ООО dСАФ-ХОЛЛАНД Русq.pdf",
                "https://kyc-compliance.ru/upload/iblock/6c4/x570ulje3oaw7vds5nag5d14dw9jk6ci/RUS_Minutes_extraordinary meeting 2022.11.03_signed.PDF",
                "https://kyc-compliance.ru/upload/iblock/0ba/r002fdgqzzkeaxw3kfjd2p3qlapy8tv8/egrul.pdf",
                "https://kyc-compliance.ru/upload/iblock/9ce/m6vu7h1ax0i1u0moyd8ykdseh88fdloy/beneficiar.pdf"
            };

            Console.WriteLine($"Запущено {maxParallelTask} поток(а/ов):");
            await DownloadFiles(maxParallelTask, sFiles, sPathFiles);

            var stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine("<<");

            //Console.ReadKey();
            Console.WriteLine(
                $"время выполнения {(DateTimeOffset.FromUnixTimeMilliseconds(stop) - DateTimeOffset.FromUnixTimeMilliseconds(start))} секунд");
            // Console.WriteLine((dateTimeOffsetStart).ToString("yyyy-MM-dd HH:mm:ss.fff")); Console.WriteLine((dateTimeOffsetStop).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public static async Task DownloadFiles(int maxParallelDownload, string[] urls, string filePaths)
        {
            FileDownloader
                downloader = new FileDownloader(maxParallelDownloads: maxParallelDownload); // Ограничить поток
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
                Boolean statisticProgress = true;
                Boolean statisticFileCompleted = false;

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
                                    long existingFileSize = 0;
                                    try
                                    {
                                        existingFileSize = new FileInfo(filePath).Length;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Сообщение: {e.Message}");
                                        //throw;
                                    }
                                    // Добавляем заголовок Range в запросе для возобновления закачки с указанной позиции
                                    client.Headers.Add("Content-Range", $"bytes {existingFileSize}-");
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
                Console.WriteLine("Закачано {0} байт из общего размера {1} байт", e.BytesReceived,
                    e.TotalBytesToReceive);
            }
        }
    }
}