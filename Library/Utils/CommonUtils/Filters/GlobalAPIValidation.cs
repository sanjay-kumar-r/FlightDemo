﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CommonDTOs;
using ServiceContracts.CustomException;
using Microsoft.Extensions.Configuration;
using Utils.Logger;
using System.Diagnostics;
using ServiceContracts.Logger;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CommonUtils.Filters
{
    public class GlobalAPIValidation : Attribute, IAsyncActionFilter, IExceptionFilter
    {
        private static HeaderInfo _headerInfo = null;
        private readonly IConfiguration config;
        private readonly ICustomExceptionMessageBuilder exBuilder;
        private readonly ILogger logger;
        private Stopwatch timer = null;

        public GlobalAPIValidation(IConfiguration config, ICustomExceptionMessageBuilder exBuilder, ServiceContracts.Logger.ILogger logger)
        {
            this.config = config;
            this.exBuilder = exBuilder;
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            SetHeaderInfo(context);
            SetLogicalThreadContextAndLogEntry(context);
           
            timer = Stopwatch.StartNew();
            if (await ValidateRequest(context))
            {
                await next();
            }
            else
            {
                timer.Stop();
                throw new Exception("User is unauthorized");
            }
            string operationName = context.RouteData.Values["controller"] + "->" + context.RouteData.Values["action"];
            timer.Stop();
            logger.Log(LogLevel.INFO,
                $"API '{operationName}' execution Ended => timeElapsed = '{timer?.Elapsed.TotalMilliseconds}'ms");
        }

        public void OnException(ExceptionContext context)
        {
            if (exBuilder.Messages != null && exBuilder.Messages.Count() > 0)
            {
                foreach (var msg in exBuilder.Messages)
                {
                    logger.Log(LogLevel.ERROR, msg);
                }
            }
            logger.Log(LogLevel.ERROR, $"errorMessage :=>'{context.Exception.Message}' stackTrace :=>'[{context.Exception.StackTrace}]'");
            timer.Stop();
            logger.Log(LogLevel.INFO,
                $"Execution Ended => timeElapsed = '{timer?.Elapsed.TotalMilliseconds}'ms");
            //throw new Exception("Internal_Setver_Error");
            throw context.Exception;
        }

        private void SetHeaderInfo(ActionExecutingContext context)
        {
            var header = new HeaderInfo();
            header.UserId = context.HttpContext.Request.Headers["UserId"];
            header.TenantId = context.HttpContext.Request.Headers["TenantId"];
            header.AccessToken = context.HttpContext.Request.Headers["AccessToken"];
            _headerInfo = header;
        }

        private void SetLogicalThreadContextAndLogEntry(ActionExecutingContext context)
        {
            //string traceId = Activity.Current?.RootId ?? context?.HttpContext?.TraceIdentifier;
            string traceId = context?.HttpContext?.TraceIdentifier;
            string spanId = Activity.Current?.SpanId.ToString();
            //string operationName = context.Controller.ToString() + " -> " + context.ActionDescriptor.DisplayName;
            string operationName = context.RouteData.Values["controller"] + "->" + context.RouteData.Values["action"];
            LoggerUtils.SetLogicalThreadContext(spanId, traceId, operationName, _headerInfo.UserId, _headerInfo.TenantId);

            logger.Log(LogLevel.INFO, 
                $"Entering API '{operationName}' with request :=> [{JsonConvert.SerializeObject(context.ActionArguments)}] " +
                $"and requestType :=> [{context.HttpContext.Request.Method}]");
        }

        private async Task<bool> ValidateRequest(ActionExecutingContext context)
        {
            //if (CheckIsValidationRequired(config.GetSection("HeaderValidation"), context.Controller.ToString(),
            //    context.ActionDescriptor.DisplayName))
                bool isValid = true;
            if (CheckIsValidationRequired(config.GetSection("HeaderValidation"), context.RouteData.Values["controller"].ToString(),
                context.RouteData.Values["action"].ToString(), context.HttpContext.Request.Path))
                isValid = ValidateHeader(_headerInfo);
            else
                logger.Log(LogLevel.INFO, "Header validation skipped.");
            if (isValid)
            {
                if (CheckIsValidationRequired(config.GetSection("AdminValidation"), context.RouteData.Values["controller"].ToString(),
                    context.RouteData.Values["action"].ToString(), context.HttpContext.Request.Path))
                    isValid = await ValidateAdmin(context);
                else
                    logger.Log(LogLevel.INFO, "Admin validation skipped.");
            }
            return isValid;
        }

        private bool CheckIsValidationRequired(IConfigurationSection configurationSection, string controllerName , string actionName, 
            string apiPath = null)
        {
            if (configurationSection != null)
            {
                if (configurationSection.GetSection("ISValidationRequired") != null
                    && Convert.ToBoolean(configurationSection.GetSection("ISValidationRequired").Value))
                {
                    //check if any excludeControllers list matches request controllerName 
                    var excludeControllersSection = configurationSection.GetSection("ExcludeControllers");
                    if (excludeControllersSection != null && !string.IsNullOrWhiteSpace(excludeControllersSection.Value))
                    {
                        List<string> excludeControllers = excludeControllersSection.Value.Split(',').ToList();
                        if (excludeControllers.Any(x => x.Equals(controllerName, StringComparison.OrdinalIgnoreCase)))
                            return false;
                    }

                    //check if any excludeActions list match request actionName (or ControllerName:ActionName)
                    var excludeActionsSection = configurationSection.GetSection("ExcludeActions");
                    if (excludeActionsSection != null && !string.IsNullOrWhiteSpace(excludeActionsSection.Value))
                    {
                        var excludeActions = excludeActionsSection.Value.Split(',');
                        bool isExcludeValidation = excludeActions.Any(x =>
                        {
                            //match with ControllerName:ActionName
                            if (x.Contains(':'))
                            {
                                var lst = x.Split(':');
                                return lst[0].Equals(controllerName, StringComparison.OrdinalIgnoreCase)
                                    && lst[1].Equals(actionName, StringComparison.OrdinalIgnoreCase);
                            }
                            else
                            {
                                //match with just ActionName
                                return x.Equals(actionName, StringComparison.OrdinalIgnoreCase);
                            }
                        });
                        if (isExcludeValidation)
                            return false;
                    }
                    if (!string.IsNullOrWhiteSpace(apiPath))
                    {
                        //check if any ExcludeApiPathSection list matches request apiPath 
                        var ExcludeApiPathSection = configurationSection.GetSection("ExcludeApiPath");
                        if (ExcludeApiPathSection != null)
                        {
                            var excludeControllers = ExcludeApiPathSection.AsEnumerable().ToList();
                            if (excludeControllers.Any(x =>
                                x.Value != null && x.Value.Trim(' ', '/').Equals(apiPath.Trim(' ', '/'), StringComparison.OrdinalIgnoreCase)))
                                return false;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        private bool ValidateHeader(HeaderInfo headerInfo)
        {
            string message = string.Empty;
            if (String.IsNullOrWhiteSpace(headerInfo.UserId))
                message += "UserId, ";
            if (String.IsNullOrWhiteSpace(headerInfo.TenantId))
                message += "TenantId, ";
            if (String.IsNullOrWhiteSpace(headerInfo.AccessToken))
                message += "AccessToken";

            if (string.IsNullOrWhiteSpace(message))
                return true;
            else
            {
                //exBuilder.AddMessage($"Required Header fileds missing :'{message}'");
                logger.Log(LogLevel.ERROR, $"Required Header fileds missing : '{message}'");
                return false;
            }
        }

        private async Task<bool> ValidateAdmin(ActionExecutingContext context)
        {
            bool isValid = false;
            using(var httpClient = new HttpClient())
            {
                if(config.GetSection("APIGatewayUrl") != null && !string.IsNullOrWhiteSpace(config.GetSection("APIGatewayUrl").Value))
                {
                    string apiGatewaUrl = config.GetSection("APIGatewayUrl").Value;
                    if(config.GetSection("endpointUrls") != null && config.GetSection("endpointUrls").GetSection("ValidateAdminUrl") != null 
                        && !string.IsNullOrWhiteSpace(config.GetSection("endpointUrls").GetSection("ValidateAdminUrl").Value))
                    {
                        string ValidateAdminUrl = config.GetSection("endpointUrls").GetSection("ValidateAdminUrl").Value;
                        string requestUrl = apiGatewaUrl.Trim('/', ' ') + "/" + ValidateAdminUrl.Trim('/', ' ');
                        //StringContent content = new StringContent(JsonConvert.SerializeObject(myClassObject), Encoding.UTF8, "application/json");
                        StringContent requestBody = new StringContent(_headerInfo.UserId, Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PostAsync(requestUrl, requestBody))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            isValid = Convert.ToBoolean(apiResponse);
                        }
                    }
                }
            }
            if (!isValid)
            {
                //exBuilder.AddMessage("Admin validation returned 'false'");
                logger.Log(LogLevel.ERROR, "Admin validation returned 'false'");
            }
            return isValid;
        }

    }
}
