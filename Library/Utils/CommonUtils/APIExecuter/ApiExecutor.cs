using AuthDTOs;
using CommonDTOs;
using Newtonsoft.Json;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtils.APIExecuter
{
    public class ApiExecutor
    {
        private readonly ILogger logger;

        public ApiExecutor(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<HttpResponseMessage> CallAPIWithRetry(APIRequestType requestType, string requestUrl, HeaderInfo headerInfo,
            object requestBody = null, string refreshTokenUrl = null, bool isAuthorizatonRequired = false)
        {
            logger.Log(LogLevel.INFO, $"Entering method CallAPIWithRetry:=> requestUrl = '{requestUrl}', requestType = '{requestType.ToString()}', "
                + $"headerInfo = '{JsonConvert.SerializeObject(headerInfo)}', requestBody = '{JsonConvert.SerializeObject(requestBody)}' ,"
                + $"refreshTokenUrl = '{refreshTokenUrl}'");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                logger.Log(LogLevel.INFO, $"CallAPI initiated : with requestUrl = '{requestUrl}'");
                response = await CallAPI(requestType, requestUrl, headerInfo, requestBody, isAuthorizatonRequired);
                //if(response.StatusCode == HttpStatusCode.OK)
                //{
                //    //return response;
                //}
                if((response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    && !string.IsNullOrWhiteSpace(refreshTokenUrl) && !string.IsNullOrWhiteSpace(headerInfo.RefreshToken)
                    && isAuthorizatonRequired)
                {
                    logger.Log(LogLevel.ERROR, $"RefreshTokenAndCallAPI initiated : with requestUrl = '{requestUrl}', refreshTokenUrl = '{refreshTokenUrl}'");
                    response = await RefreshTokenAndCallAPI(requestType, requestUrl, headerInfo, requestBody, refreshTokenUrl, isAuthorizatonRequired);
                }
            }
            catch(UnauthorizedAccessException ex)
            {
                logger.Log(LogLevel.ERROR, $"UnauthorizedAccessException ocured in CallAPIWithRetry. ErrorMessage = '{ex.Message}'," +
                    $" StachTrace = '{ex.StackTrace}'");
                if (!string.IsNullOrWhiteSpace(refreshTokenUrl) && !string.IsNullOrWhiteSpace(headerInfo.RefreshToken)
                    && isAuthorizatonRequired)
                {
                    logger.Log(LogLevel.ERROR, $"RefreshTokenAndCallAPI initiated on catch(UnauthorizedAccessException) : " +
                        $"with requestUrl = '{requestUrl}', refreshTokenUrl = '{refreshTokenUrl}'");
                    response = await RefreshTokenAndCallAPI(requestType, requestUrl, headerInfo, requestBody, refreshTokenUrl, isAuthorizatonRequired);
                }
                else
                {
                    logger.Log(LogLevel.ERROR, $" refreshTokenUrl is null or empty. Therefore skipping RefreshTokenAndCallAPI()");
                    throw ex;
                }
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.ERROR, $"Exception ocured in CallAPIWithRetry. ErrorMessage = '{ex.Message}', " +
                    $"StachTrace = '{ex.StackTrace}'");
                throw ex;
            }
            logger.Log(LogLevel.INFO, $"CallAPIWithRetry endedn with response : {await response.Content.ReadAsStringAsync()}");
            return response;
        }
        public async Task<HttpResponseMessage> RefreshTokenAndCallAPI(APIRequestType requestType, string requestUrl,
            HeaderInfo headerInfo, object requestBody, string refreshTokenUrl, bool isAuthorizatonRequired = false)
        {
            logger.Log(LogLevel.INFO, $"Entering method RefreshTokenAndCallAPI:=> requestUrl = '{requestUrl}', requestType = '{requestType.ToString()}', "
                + $"headerInfo = '{JsonConvert.SerializeObject(headerInfo)}', requestBody = '{JsonConvert.SerializeObject(requestBody)}' ,"
                + $"refreshTokenUrl = '{refreshTokenUrl}'");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                var refreshTokenBody = new RefreshTokenRequest()
                {
                    RefreshToken = headerInfo.RefreshToken,
                    ExpiredToken = headerInfo.Authorization.Split(" ").LastOrDefault()
                };
                logger.Log(LogLevel.INFO, $"CallAPI initiated for refreshtoken : with refreshTokenUrl = '{refreshTokenUrl}' , " +
                    $"refreshTokenBody = '{JsonConvert.SerializeObject(refreshTokenBody)}'");
                response = await CallAPI(requestType, refreshTokenUrl, headerInfo, refreshTokenBody, isAuthorizatonRequired);
                logger.Log(LogLevel.INFO, $"CallAPI ended for refreshtoken : with refreshResponse = " +
                    $"'{await response.Content.ReadAsStringAsync()}' ");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var refreshTokenResponse = JsonConvert.DeserializeObject<AuthResponse>(apiResponse);
                    if(refreshTokenResponse != null && refreshTokenResponse.IsSuccess)
                    {
                        headerInfo.Authorization = "Bearer " + refreshTokenResponse.Token;
                        logger.Log(LogLevel.INFO, $"CallAPI initiated  : with requestUrl = '{requestUrl}' , " +
                            $"requestBody = '{JsonConvert.SerializeObject(requestBody)}'");
                        response = await CallAPI(requestType, requestUrl, headerInfo, requestBody, isAuthorizatonRequired);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, $"Exception ocured in RefreshTokenAndCallAPI. ErrorMessage = '{ex.Message}', " +
                    $"StachTrace = '{ex.StackTrace}'");
                throw ex;
            }
            logger.Log(LogLevel.INFO, $"RefreshTokenAndCallAPI ended  : with response = '{await response.Content.ReadAsStringAsync()}'");
            return response;
        }

        public async Task<HttpResponseMessage> CallAPI(APIRequestType requestType, string requestUrl, HeaderInfo headerInfo,
            object requestBody, bool isAuthorizatonRequired = false)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                if (isAuthorizatonRequired)
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", headerInfo.Authorization);
                    httpClient.DefaultRequestHeaders.Add("RefreshToken", headerInfo.RefreshToken);
                }

                //if (isAuthorizationRequired)
                //{
                //    string username = "admin";
                //    string password = "admin";
                //    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                //    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                //}
                switch (requestType)
                {
                    case APIRequestType.Get:
                        {
                            response = await httpClient.GetAsync(requestUrl);
                            break;
                        }
                    case APIRequestType.Post:
                        {
                            response = await httpClient.PostAsJsonAsync(requestUrl, requestBody);
                            break;
                        }
                    case APIRequestType.Delete:
                        {
                            response = await httpClient.DeleteAsync(requestUrl);
                            break;
                        }
                    default:
                        break;
                }
                return response;
            }
        }

        public async Task<HttpResponseMessage> ExecuteDeleteAPI(string requestUrl, HeaderInfo headerInfo, bool isAuthorizationRequired = false)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                httpClient.DefaultRequestHeaders.Add("Authorization", headerInfo.Authorization);
                //if (isAuthorizationRequired)
                //{
                //    string username = "admin";
                //    string password = "admin";
                //    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                //    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
                //}
                var response = await httpClient.DeleteAsync(requestUrl);
                return response;
            }
        }

        public async Task<HttpResponseMessage> ExecuteGetAPI(string requestUrl, HeaderInfo headerInfo)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(requestUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("UserId", headerInfo.UserId);
                httpClient.DefaultRequestHeaders.Add("TenantId", headerInfo.TenantId);
                httpClient.DefaultRequestHeaders.Add("Authorization", headerInfo.Authorization);
                //HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                var response = await httpClient.GetAsync(requestUrl);
                return response;
            }
        }
    }

    public enum APIRequestType
    {
        Get,
        Post,
        Delete,
        Put
    }
}
