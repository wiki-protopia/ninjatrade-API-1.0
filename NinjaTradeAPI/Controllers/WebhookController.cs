using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebhookReceiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement payload)
        {
            // Get initial date here
            string alert = payload.GetProperty("alert").GetString();
            string account = payload.GetProperty("account").GetString();
            string ticker = payload.GetProperty("ticker").GetString();
            string stop_price = payload.GetProperty("stop_price").GetString();
            string tif = payload.GetProperty("tif").GetString();
            string oco_id = payload.GetProperty("oco_id").GetString();

            // Process the webhook payload here
            int qty = payload.GetProperty("qty").GetInt32();

            string propertyName = null, propertyValue = null;

            for (int i = 1; i <= qty; i++)
            {
                propertyName = $"take_profit_{i}_price";
                Console.WriteLine(propertyName);
                propertyValue = payload.GetProperty(propertyName).GetString();

                var userData = new Dictionary<string, object>
                {
                    { "alert", alert },
                    { "account", account },
                    { "ticker", ticker },
                    { "qty", 1 },
                    { propertyName, propertyValue },
                    { "stop_price", stop_price },
                    { "tif", tif },
                    { "oco_id", oco_id }
                };
                string jsonData = JsonSerializer.Serialize(userData);
                Console.WriteLine(userData.ToString());
            }

            var breakData = new Dictionary<string, object>
            {
                { "alert", "Adjusted OCO Short" },
                { "account", account },
                { "ticker", ticker },
                { "qty", qty },
                { propertyName, propertyValue },
                { "stop_price", stop_price },
                { "tif", tif },
                { "oco_id", "Same trade num for TP2 OCO_ID" }
            };

            string breakjsonData = JsonSerializer.Serialize(breakData);
            Console.WriteLine(breakjsonData.ToString());

            // Respond with a 200 OK status code
            return Ok();
        }
    }
}