using System;
using System.Net.Http.Headers;

namespace ExempleAnketaKYCService.Settings
{
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