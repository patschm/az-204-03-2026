using ACME.Web.Calculator.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ACME.Web.Calculator.Controllers;

public class CalculatorController(IHttpClientFactory httpClientFactory, DaprClient dapr, IConfiguration config) : Controller
{
    
    public IActionResult Index(CalculatorModel? model = null)
    {
        if (model == null) model = new CalculatorModel();
        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(CalculatorModel model)
    {
        //var client = httpClientFactory.CreateClient("AddService");
        var daprid = config["CalculatorService:Dapr:AppId"];
        var client = dapr.CreateInvokableHttpClient(daprid);
        var result = await client.GetAsync($"add?a={model.A}&b={model.B}");
        if (result.IsSuccessStatusCode)
        {
            model.Result = await result.Content.ReadFromJsonAsync<int>();
        }
        
        return View("Index", model);
    }
}
