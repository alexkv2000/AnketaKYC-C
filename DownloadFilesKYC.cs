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
        public static async Task Main(string[] args)
        {
            int maxParallelTask = 4;
            String sPathFiles = "c:/Downloads/36419/";
            Directory.CreateDirectory(sPathFiles);
            long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine($">> {start}");

            String[] sFiles =
            {
                "https://kyc-compliance.ru/upload/iblock/137/u7sv9obtj72umaddo57vfveg6k9pujpe/ОГРН (3).pdf",
                "https://kyc-compliance.ru/upload/iblock/6af/rgzg94xouogch1xb79qyq8pyr0mqp98f/Выписка из ЕГРЮЛ 09.11.2023.pdf",
                "https://kyc-compliance.ru/upload/iblock/c08/t9gr4t6lq6wn8iu6kio72bdpgjhsit8u/3. Устав ТЛК-Групп (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/6a0/4x9l6ndr898mqxck7n1tdi78y042wj9y/Spravka_01-11-2023.pdf",
                "https://kyc-compliance.ru/upload/iblock/f1b/ar3eo4e33y9dl4e1xcwb64ezmp3k4xpf/7. Бух. баланс 2022 год.pdf",
                "https://kyc-compliance.ru/upload/iblock/2a8/fxddgfew0p9it6r8whopkze2x1gj1zvb/Карта ООО ТЛК-Групп (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/88e/1p4xyiu8ghkvwukoxjr1iukefhwzc96z/5. Приказ _ 1 (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/88e/1p4xyiu8ghkvwukoxjr1iukefhwzc96z/5. Приказ _ 1 (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/88e/1p4xyiu8ghkvwukoxjr1iukefhwzc96z/5. Приказ _ 1 (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/0eb/nd0tpwrmnoi4zohr2bmvqxpj4432be13/4. Решение _ 4 (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/d43/ry2ej55rhvydles26cobbi2l8ihl1fy0/4.1 Решение _ 5 (1).pdf",
                "https://kyc-compliance.ru/upload/iblock/9df/xza7uf0bq9kgifwxazh0l5x3uun3ucy3/ИНН (9).pdf",
                "https://kyc-compliance.ru/upload/iblock/ea3/5z8xtmyaf3psu9s25023flroomrnwc44/doc04173720231107100611.pdf",
                "https://kyc-compliance.ru/upload/iblock/56d/2axemmu52s3c3xp1uurhxzgs52ecni98/Приказ о внесении изменений в связи со сменой фамилии.pdf",
                "https://kyc-compliance.ru/upload/iblock/745/ew2ktgvoc64fl33lyuh7gj9b7exay8gp/Приказ _2 Полякова Е.В. (13).pdf",
                "https://kyc-compliance.ru/upload/iblock/926/db4voto14rx6u6vkjdhn1byqzotkx361/Штатное расписание 2023 года.pdf",
                "https://kyc-compliance.ru/upload/iblock/cc6/28h3inxqozuy05p3ogeafvtkbi3hq0un/Полис ТЛК-Групп (10).pdf",
                "https://kyc-compliance.ru/upload/iblock/5d6/u0nnza3x7p47jhoguxb205e6mg142v4g/Схема проверки (2).pdf",
                "https://kyc-compliance.ru/upload/iblock/762/sc9i3a198gd22ffi1xhod260qj617i54/ООО ДЕАЛ 11 22 Групп (4).pdf",
                "https://kyc-compliance.ru/upload/iblock/bb4/mvwh55d6wdsu3yoalbejeijmzo2ll1ky/ИП Галкин А.Н. 1 22 Групп (4).pdf",
                "https://kyc-compliance.ru/upload/iblock/23e/ys0bvevbgqmly636wompls5sl1r3mds0/ИП Галкин А.Н. 3 22 Групп (3).pdf",
                "https://kyc-compliance.ru/upload/iblock/02b/wo1cqa3fjk45cn78afrykjweu9vigmal/ИП Галкин А.Н. 13 22 Групп (3).pdf",
                "https://kyc-compliance.ru/upload/iblock/004/vlk56yp09znwlz45wpwnolz83e1g4u09/ИП Дугинов А.Н. 9 22 Групп.pdf",
                "https://kyc-compliance.ru/upload/iblock/b0a/5yazo4teatyhhx8iboem2e7qufhhq5vj/ИП Филиппов Е.В. 5 22 Групп.pdf",
                "https://kyc-compliance.ru/upload/iblock/3a3/20ywh1hz067atrgslx5pp6unosajx89p/ИП Филиппов Е.В. 7 22 Групп.pdf",
                "https://kyc-compliance.ru/upload/iblock/819/2rvx4dhgsx2qfs41fi5rvfpr42xi137m/10. Договор аренды офиса.pdf",
                "https://kyc-compliance.ru/upload/iblock/cb4/mfnsbn0b3gtqrag8gv8njcts4g7sw91a/Штатное расписание 2023 (8).pdf",
                "https://kyc-compliance.ru/upload/iblock/f2e/ati70vrkprkrvrtnnay5v3lhilnc7m3z/Справка об исполнении обязанности по уплате налогов, сборов, пеней, штрафов на 26.09.23 ООО _ТЛК-ГРУПП_ (6950198198-695001001) (6).pdf",
                "https://kyc-compliance.ru/upload/iblock/c3a/r5hgefgt3fcdnfsz4po4h30mk3z5f3d9/Расчет налоговой нагрузки за 2022 год..pdf",
                "https://kyc-compliance.ru/upload/iblock/f1b/ar3eo4e33y9dl4e1xcwb64ezmp3k4xpf/7. Бух. баланс 2022 год.pdf",
                "https://kyc-compliance.ru/upload/iblock/c1a/an7h3k6r4ulymk2jiqlpfo9dldmcsnb0/анкета тендер.pdf",
                "https://kyc-compliance.ru/xls_files/ques_36419.xls",
                "https://kyc-compliance.ru/upload/iblock/e62/625q53lg1ip70g9i1ctdtbz813t2eq8s/all_sanctions_cp_Q_36419.pdf"
            };

            Console.WriteLine($"Запущено {maxParallelTask} поток(а/ов):");
            //уникальный Map по имени файла
            Dictionary<string,string> fileMap = new Dictionary<string, string>();
            foreach (string path in sFiles)
            {
                string fileName = System.IO.Path.GetFileName(path);
                if (!fileMap.ContainsKey(fileName))
                {
                    fileMap.Add(fileName,path);
                }
            }

            Console.WriteLine();
            //await DownloadFiles(maxParallelTask, sFiles, sPathFiles);
            await DownloadFiles(maxParallelTask, fileMap, sPathFiles);

            var stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(
                $"время выполнения {(DateTimeOffset.FromUnixTimeMilliseconds(stop) - DateTimeOffset.FromUnixTimeMilliseconds(start))} секунд");
            Console.WriteLine("<<");

            //Console.ReadKey();
            Console.WriteLine(
                $"время выполнения {(DateTimeOffset.FromUnixTimeMilliseconds(stop) - DateTimeOffset.FromUnixTimeMilliseconds(start))} секунд");
            // Console.WriteLine((dateTimeOffsetStart).ToString("yyyy-MM-dd HH:mm:ss.fff")); Console.WriteLine((dateTimeOffsetStop).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public static async Task DownloadFiles(int maxParallelDownload, Dictionary<string,string> urls, string filePaths)
        {
            FileDownloader downloader = new FileDownloader(maxParallelDownloads: maxParallelDownload); // Ограничить поток

                await downloader.DownloadFilesWithLimitedParallelism(urls, filePaths).ConfigureAwait(true);

            
        }

        public class FileDownloader
        {
            private SemaphoreSlim semaphore;
            public FileDownloader(int maxParallelDownloads)
            {
                semaphore = new SemaphoreSlim(maxParallelDownloads);
            }
            public async Task DownloadFilesWithLimitedParallelism(Dictionary<string, string> urls, string pathFile)
            {
                Boolean statisticProgress = false;
                Boolean statisticFileCompleted = true;
               
                var array = urls.Values.ToArray();
                Task[] downloadTasks = new Task[array.Length];
                
                for (int i = 0; i < array.Length; i++)
                {
                    string url = array[i];
                    string filePath = pathFile + Path.GetFileName(array[i]);

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

            void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
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

                 void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                Console.WriteLine("Закачано {0} байт из общего размера {1} байт", e.BytesReceived,
                    e.TotalBytesToReceive);
            }
        }
    }
}
