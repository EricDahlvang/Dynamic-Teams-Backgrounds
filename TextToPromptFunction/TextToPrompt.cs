using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static TextToPromptFunction.TextToPromptInput;

namespace TextToPromptFunction
{
    public static class TextToPrompt
    {
        [FunctionName("TextToPrompt")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("TextToPrompt running");
                var config = new ConfigurationBuilder()
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddJsonFile("local.settings.json", true)
                            .AddEnvironmentVariables()
                            .Build();

                string azureKey = config.GetValue<string>("AzureKeyCredential");

                // 
                // Used to temporarily allow GET and POST params @@TODO Only POST is working right now
                //
                var parms = await ParseParams(req);

                //
                // Get Important Sentence, Entities, and Sentiment 
                // 
                var textAnalyticsResult = await SubjectPrompt.Parse(azureKey, parms.Text, parms.MinConfidenceScore);


                var inputForPrompt = parms.PromptContentType == PromptContentTypes.rawText ? textAnalyticsResult.Text :
                       textAnalyticsResult.Entities.Count != 0 ? textAnalyticsResult.Entities : textAnalyticsResult.Text;

                //
                // Just do a pos/neg range (I may not be grokking positive/negative fields usage here...)
                //
                var decorations = StableDiffusionDecoration.CreateRandomDecoratorsBasedOnSentiment(textAnalyticsResult.Positive, textAnalyticsResult.Negative, textAnalyticsResult.Neutral);

                var deocrationsList = string.Join(", ", decorations);


                var emphasis = inputForPrompt.Select(s => $"(({s}))"); // Force emphasis 

                var subjectList = string.Join(" and ", emphasis);

                var finalPrompt = $"{subjectList}, {deocrationsList}";

                textAnalyticsResult.Prompt = finalPrompt;

                var result = JsonConvert.SerializeObject(textAnalyticsResult, Formatting.Indented);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(result, Encoding.UTF8, "application/json")
                };
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message, Encoding.UTF8, "application/json")
                };

            }
        }

        private static async Task<TextToPromptInput> ParseParams(HttpRequest req)
        {
            //@@TODO: Not working now - only will work if body passed (so only POST)
            if (req.Body.Length > 0) // Better way to determine verb? 
            {
                string json = await req.ReadAsStringAsync();
                var input = JsonConvert.DeserializeObject<TextToPromptInput>(json);

                return input;
            }
            else
            {
                return new TextToPromptInput()
                {
                    Text = req.Query["text"],
                    MinConfidenceScore = Double.Parse(req.Query["minConfidenceScore"]),
                    PromptContentType = Enum.Parse<PromptContentTypes>(req.Query["PromptContentType"])
                };
            }
        }
    }
}

