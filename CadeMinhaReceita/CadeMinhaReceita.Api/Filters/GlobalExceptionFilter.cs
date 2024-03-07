using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CadeMinhaReceita.Api.Models;
using CadeMinhaReceita.Domain.Extensions;

namespace CadeMinhaReceita.Api.Filters
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is FlurlHttpException flurlException)
            {
                await FlurExceptionTreatment(context, flurlException);
            }
            else
            {
                SetJsonResult(context, StatusCodes.Status500InternalServerError, new ErrorResponse(context.Exception.Message), LogLevel.Error);
            }
        }

        private async Task FlurExceptionTreatment(ExceptionContext context, FlurlHttpException flurlException)
        {
            var errorMessage = string.Empty;

            var errorResponseObject = new ErrorResponse();

            try
            {
                errorMessage = await flurlException.GetResponseStringAsync() ?? context.Exception.Message;

                errorResponseObject = new ErrorResponse(errorMessage);

                SetBadRequest(context, errorResponseObject);
            }
            catch (Flurl.Http.FlurlParsingException ex)
            {
                SetBadRequest(context, errorResponseObject);
            }
            catch (Exception ex)
            {
                SetJsonResult(context, StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message), LogLevel.Error);
            }
        }

        private void SetBadRequest(ExceptionContext context, ErrorResponse errorResponse)
        {
            var contextMessage = string.Empty;

            if (errorResponse.Messages.Any())
            {
                contextMessage = errorResponse.Messages.Count > 1 ? string.Join(",", errorResponse.Messages) : errorResponse.Messages.FirstOrDefault();
            }

            context.Exception = !string.IsNullOrEmpty(contextMessage) ? new Exception(contextMessage) : context.Exception;

            SetJsonResult(context, StatusCodes.Status400BadRequest, errorResponse, LogLevel.Error);
        }

        private void SetJsonResult(ExceptionContext context, int status, ErrorResponse error, LogLevel logLevel)
        {
            WriteLog(context, logLevel);
            context.Result = new JsonResult(error) { StatusCode = status };
        }

        private void WriteLog(ExceptionContext context, LogLevel logLevel)
        {
            const string logTemplate = "{requestProtocol} {method} {requestPath}";

            if (logLevel == LogLevel.Information)
            {
                _logger.LogInformation(context.Exception, logTemplate, context.HttpContext.Request.Protocol, context.HttpContext.Request.Method, context.HttpContext.Request.Path);
            }
            else if (logLevel == LogLevel.Warning)
            {
                _logger.LogWarning(context.Exception, logTemplate, context.HttpContext.Request.Protocol, context.HttpContext.Request.Method, context.HttpContext.Request.Path);
            }
            else if (logLevel == LogLevel.Error)
            {
                _logger.LogError(context.Exception, logTemplate, context.HttpContext.Request.Protocol, context.HttpContext.Request.Method, context.HttpContext.Request.Path);
            }
        }
    }
}
