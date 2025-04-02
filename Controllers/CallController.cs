
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Core;
using AiLeadBot.Services;

namespace AiLeadBot.Controllers
{
    [Route("call")]
    public class CallController : TwilioController
    {
        private readonly GptService _gpt;
        private readonly ElevenLabsService _tts;

        public CallController(GptService gpt, ElevenLabsService tts)
        {
            _gpt = gpt;
            _tts = tts;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string SpeechResult)
        {
            var input = string.IsNullOrWhiteSpace(SpeechResult) ? "Hello" : SpeechResult;
            var gptResponse = await _gpt.GetReplyAsync(input);
            var audioUrl = await _tts.GenerateAudioUrlAsync(gptResponse);

            var response = new VoiceResponse();

            // 1. Create the Gather block
            var gather = new Twilio.TwiML.Voice.Gather(
                 input: new[] { Twilio.TwiML.Voice.Gather.InputEnum.Speech },
                 action: new Uri("/call", UriKind.Relative),
                 method: "POST",
                 timeout: 5
             );

            // 2. Add TTS or audio inside the Gather
            gather.Play(new Uri(audioUrl));

            // 3. Append the Gather to the main VoiceResponse
            response.Append(gather);

            // 4. Add a fallback Say for when no input is received
            response.Say("Sorry, I didn’t hear anything. Please try again.");

            // Done!
            return TwiML(response);

        }
    }
}