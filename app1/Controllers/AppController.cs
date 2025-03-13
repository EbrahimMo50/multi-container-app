using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

// direct messaging between two apps
namespace app1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    [HttpPost("send-message")]
    public async Task<IActionResult> Post(string value)
    {
        HttpClient client = new()
        {
            BaseAddress = new("http://app2:80")
        };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Console.WriteLine("this message is being sent to app 2");

        var response = await client.PostAsync($"api/App/recieve-message?message={value}", null);
        return Ok();
    }

    [HttpGet("get-saluation")]
    public string Get()
    {
        Console.WriteLine("someone invoked from outter app");
        return "Hello from app1";
    }

    [HttpPost("recieve-message")]

    public IActionResult RecieveMessage(string message)
    {
        Console.WriteLine($"recieved message {message}");
        return Ok();
    }

    [HttpGet("/invoke-other-app")]
    public async Task<IActionResult> InvokeoOtherApp()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://app2:80");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"api/App/get-saluation");
        Console.WriteLine(response);
        return Ok(response.Content);
    }
}
