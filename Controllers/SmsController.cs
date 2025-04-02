
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Core;
using AiLeadBot.Services;

namespace AiLeadBot.Controllers
{
    [Route("sms")]
    public class SmsController : TwilioController
    {
        private readonly GptService _gpt;

        public SmsController(GptService gpt)
        {
            _gpt = gpt;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string Body)
        {
            var reply = await _gpt.GetReplyAsync(Body);
            var response = new MessagingResponse();
            response.Message(reply);

            return TwiML(response);
        }
    }
}