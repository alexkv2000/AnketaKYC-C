using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExempleAnketaKYCService
{
    public class uploadFiles
    {
        static async Task Main()
        {
            // URL вашего файла
            string fileUrl = "https://kyc-compliance.ru/upload/iblock/fc8/wkqkefum64oritimkmmkzgl40z2dpn74/Анкета-НАК.pdf";

            // Путь для сохранения файла локально
            string localFilePath = "C:\\Downloads\\34477\\Анкета-НАК.pdf";

            // Позиция, с которой нужно возобновить загрузку (в байтах)
            long startPosition = 10000; // Например, начать с 1024 байт

            // Создаем экземпляр HttpClient
            using (HttpClient httpClient = new HttpClient())
            {
                // Добавляем заголовок Range для указания позиции
                httpClient.DefaultRequestHeaders.Range =
                    new System.Net.Http.Headers.RangeHeaderValue(startPosition, null);

                // Выполняем GET-запрос
                HttpResponseMessage response =
                    await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

                // Проверяем, успешен ли запрос
                if (response.IsSuccessStatusCode)
                {
                    // Открываем локальный файл для дозакачки
                    using (FileStream fileStream = new FileStream(localFilePath, FileMode.Append, FileAccess.Write))
                    {
                        // Получаем поток с данными из ответа
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            // Копируем данные из потока ответа в локальный файл
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }
                    Console.WriteLine("Дозакачка завершена.");
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
    }
}