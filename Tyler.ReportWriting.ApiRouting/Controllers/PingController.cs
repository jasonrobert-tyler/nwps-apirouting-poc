using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Tyler.ReportWriting.ApiRouting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<PingController> _logger;

        public PingController(IHttpContextAccessor accessor, ILogger<PingController> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var context = _accessor.HttpContext;
            if (context == null)
                throw new ArgumentException("Unable to retrieve the HttpContext");

            var feature = context.Features.Get<IHttpConnectionFeature>();
            var ip = feature?.LocalIpAddress?.ToString().Replace("::ffff:", string.Empty);
            var destination = context.Request.Headers.ContainsKey("destination")
                ? context.Request.Headers["destination"][0]?.Trim()
                : string.Empty;
            _logger.LogDebug($"Local ip address is {ip}, full url is {context.Request.GetDisplayUrl()}");

            string resp;
            if (string.IsNullOrEmpty(destination))
            {
                _logger.LogDebug("No destination header found");
                resp = "pong";
            }
            else if (ip == destination)
            {
                _logger.LogDebug("This is the correct destination");
                resp = "pong";
            }
            else
            {
                _logger.LogDebug($"Forwarding to {destination}");
                var client = new HttpClient { BaseAddress = new Uri($"http://{destination}:80") };
                var response = await client.GetAsync("ping");
                var source = response.Headers.GetValues("source").FirstOrDefault();
                context.Response.Headers.Add("actual-source", new StringValues(source));
                resp = await response.Content.ReadAsStringAsync();
            }

            context.Response.Headers.Add("source", new StringValues(ip));
            return Ok(resp);
        }
    }
}
