//using Microsoft.AspNetCore.Mvc;
using Diginsight.Diagnostics;
using System.Web.Http;

namespace S10_00_AspnetWebApi480.Controllers;

[RoutePrefix("api/values")]
public class ValuesController : ApiController
{
    private readonly ILogger<ValuesController> logger;

    public ValuesController()
    {
        var logger = Observability.LoggerFactory.CreateLogger<ValuesController>();
        this.logger = logger;
        
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

    }

    [HttpGet]
    [Route("getvalues")]
    public IEnumerable<string> GetValues()
    {
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger))
        {

            return new string[] { "value1", "value2" };
        }
    }

    // GET api/values/5
    public string Get(int id)
    {

        return "value";
    }

    // POST api/values
    public void Post([FromBody] string value)
    {
    }

    // PUT api/values/5
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    public void Delete(int id)
    {
    }
}