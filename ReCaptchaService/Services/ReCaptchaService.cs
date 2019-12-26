using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReCaptchaService.Services
{
    public interface IReCaptchaService
    {
        Task<bool> VerifyResponseAsync(string token);
    }

    public class ReCaptchaService : IReCaptchaService
    {
        private readonly string _secret;

        public ReCaptchaService(string secret)
        {
            _secret = secret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Verifying the user's response
        /// https://developers.google.com/recaptcha/docs/verify
        /// </remarks>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> VerifyResponseAsync(string token)
        {
            var success = false;

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_secret}&response={token}").Result;
                response.EnsureSuccessStatusCode();
                var repsonseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(repsonseBody);
                success = apiResponse.Success;
            }

            return success;
        }
    }

    [DebuggerDisplay("Success={Success}")]
    public class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public IEnumerable<string> ErrorCodes { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}
