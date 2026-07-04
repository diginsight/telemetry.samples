using B02_02_BlazorWebassemblyModel;
using Diginsight.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B02_02_BlazorWebassemblyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientConfigController : ControllerBase
    {
        private readonly ILogger<ClientConfigController> logger;
        private readonly IConfiguration configuration;

        public ClientConfigController(ILogger<ClientConfigController> logger, 
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("auth")]
        public IActionResult GetAuthConfig()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger);

            var payload = new ClientAuthConfigResponse
            {
                ClientId = configuration["BlazorClientAuth:ClientId"],
                Authority = configuration["BlazorClientAuth:Authority"],
                ValidateAuthority = bool.TryParse(configuration["BlazorClientAuth:ValidateAuthority"], out var validateAuthority)
                    ? validateAuthority
                    : null
            };

            return Ok(payload);
        }
    }
}
