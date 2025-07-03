// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using TFS.Webhooks.Core;
using TFS.Webhooks.Models;

namespace TFS.Webhooks.Controllers
{
    [Route("tfs/api/[controller]/v1")]
    [ApiController]
    public class ServiceHookEventController : Controller
    {
        private readonly ILogger<ServiceHookEventController> _logger;
        private readonly IOptionsMonitor<Dictionary<string, string>> _accountOptions;

        public ServiceHookEventController(ILogger<ServiceHookEventController> logger, IOptionsMonitor<Dictionary<string, string>> accountOptions)
        {
            _logger = logger;
            _accountOptions = accountOptions;
        }

        [Route("/")]
        [HttpGet]
        public ActionResult Index() 
        {
            return Content(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [Route("workItemChanged/{token}")]
        [HttpPost]
        public HttpResponseMessage WorkItemChanged([FromRoute] string token, [FromBody] TfsChangedRequest body)
        {
            if (body == null || body.Resource == null)
            {
                _logger.LogWarning("请求参数异常");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                SendDingDing.Send(body, token, _accountOptions.CurrentValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"请求失败:{ex}");
            }
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("workItemCreated/{token}")]
        [HttpPost]
        public HttpResponseMessage WorkItemCreated([FromRoute] string token, [FromBody] TfsCreatedRequest body)
        {
            if (body == null || body.Resource == null)
            {
                _logger.LogWarning("请求参数异常");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                SendDingDing.Send(body, token, _accountOptions.CurrentValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"请求失败:{ex}");
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}