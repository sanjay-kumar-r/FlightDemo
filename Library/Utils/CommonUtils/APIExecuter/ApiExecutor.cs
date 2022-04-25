using CommonDTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.APIExecuter
{
    public static class ApiExecutor
    {
        public static async Task<HttpResponseMessage> ExecutePostAPI(string requestUrl, HeaderInfo headerInfo, object requestBody,
            bool isAuthorizationRequired = false)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                httpClient.DefaultRequestHeaders.Add("AccessToken", headerInfo.AccessToken);
                if (isAuthorizationRequired)
                {
                    string username = "admin";
                    string password = "admin";
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                }
                //var buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody));
                //var byteContent = new ByteArrayContent(buffer);
                //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsJsonAsync(requestUrl, requestBody);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> ExecuteDeleteAPI(string requestUrl, HeaderInfo headerInfo, bool isAuthorizationRequired = false)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                httpClient.DefaultRequestHeaders.Add("AccessToken", headerInfo.AccessToken);
                if (isAuthorizationRequired)
                {
                    string username = "admin";
                    string password = "admin";
                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                }
                var response = await httpClient.DeleteAsync(requestUrl);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> ExecuteGetAPI(string requestUrl, HeaderInfo headerInfo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                client.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                client.DefaultRequestHeaders.Add("AccessToken", headerInfo.AccessToken);
                //HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                var response = await client.GetAsync(requestUrl);
                return response;
            }
        }
    }
}
