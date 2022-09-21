﻿using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextToPromptFunction
{
    public static class SubjectPrompt
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(""); //@@TODO. For now, Get Me: https://ms.portal.azure.com/#@microsoft.onmicrosoft.com/asset/Microsoft_Azure_KeyVault/Secret/https://hackathon-sd-kv.vault.azure.net/secrets/functionapp-azure-key
        private static readonly Uri endpoint = new Uri("https://textanalyticssdhack.cognitiveservices.azure.com/");

        public class SentimentImporantSentanceEntityResult
        {
            private string text = "";
            private double positive;
            private double neutral;
            private double negative;
            private List<string> entities = new List<string>();
            private string prompt = "";

            public string Text { get => text; set => text = value; }
            public double Positive { get => positive; set => positive = value; }
            public double Neutral { get => neutral; set => neutral = value; }
            public double Negative { get => negative; set => negative = value; }

            public List<string> Entities { get => entities; set => entities = value; }
            public string Prompt { get => prompt; set => prompt = value; }
        }

        public static async Task<SentimentImporantSentanceEntityResult> Parse(string text)
        {
            var client = new TextAnalyticsClient(endpoint, credentials);

            var sentimentAndImporantSentanceResults = await SentimentAndImportantSentance(client, text);
            var entities = await ExtractEntities(client, sentimentAndImporantSentanceResults.Text);
            if (entities != null)
            {
                sentimentAndImporantSentanceResults.Entities = entities.Select(item => item.Text).ToList();
            }

            return sentimentAndImporantSentanceResults;
        }

        static async Task<SentimentImporantSentanceEntityResult> SentimentAndImportantSentance(TextAnalyticsClient client, string document)
        {
            var sentimentAndImporantSentanceResult = new SentimentImporantSentanceEntityResult();

            var batchInput = new List<string>
            {
                document
            };

            TextAnalyticsActions actions = new TextAnalyticsActions()
            {
                ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction() },
                //ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction>() { new ExtractKeyPhrasesAction() }, //@@TODO - Want to use?
                AnalyzeSentimentActions = new List<AnalyzeSentimentAction>() { new AnalyzeSentimentAction() }
            };

            AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions);
            await operation.WaitForCompletionAsync();
            await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
            {
                IReadOnlyCollection<ExtractSummaryActionResult> summaryResults = documentsInPage.ExtractSummaryResults;
                IReadOnlyCollection<AnalyzeSentimentActionResult> sentimentResults = documentsInPage.AnalyzeSentimentResults;

                foreach (ExtractSummaryActionResult summaryActionResults in summaryResults)
                {
                    if (summaryActionResults.HasError)
                    {
                        HandleAnalyzeError(summaryActionResults.Error);
                        continue;
                    }

                    foreach (ExtractSummaryResult documentResults in summaryActionResults.DocumentsResults)
                    {
                        if (documentResults.HasError)
                        {
                            HandleAnalyzeError(documentResults.Error);
                            continue;
                        }

                        sentimentAndImporantSentanceResult.Text = documentResults.Sentences.First(s => s.RankScore == 1).Text;
                        Console.WriteLine($"{sentimentAndImporantSentanceResult.Text}");
                    }
                }

                foreach (AnalyzeSentimentActionResult summaryActionResults in sentimentResults)
                {
                    if (summaryActionResults.HasError)
                    {
                        HandleAnalyzeError(summaryActionResults.Error);
                        continue;
                    }

                    var sentiment = summaryActionResults.DocumentsResults[0].DocumentSentiment.ConfidenceScores;

                    sentimentAndImporantSentanceResult.Neutral = sentiment.Neutral;
                    sentimentAndImporantSentanceResult.Positive = sentiment.Positive;
                    sentimentAndImporantSentanceResult.Negative = sentiment.Negative;
                }
            }

            return (sentimentAndImporantSentanceResult);
        }

        static async Task<IEnumerable<CategorizedEntity>> ExtractEntities(TextAnalyticsClient client, string document)
        {
            try
            {
                var minConfidenceScore = .10;
                var response = await client.RecognizeEntitiesAsync(document);
                CategorizedEntityCollection entitiesInDocument = response.Value;

                var interestingNER = new[] {
                    EntityCategory.Location,
                    EntityCategory.Organization,
                    EntityCategory.Person,
                    EntityCategory.PersonType,
                    EntityCategory.Product,
                    EntityCategory.Skill
                };

                var result = entitiesInDocument
                    .Where(t => interestingNER.Contains(t.Category))
                    .Where(t => t.ConfidenceScore > minConfidenceScore)
                    .GroupBy(t => t.Category)
                    .Select(group => group.First())
                    .ToList()
                    .DefaultIfEmpty();

                return result;
            }
            catch (RequestFailedException exception)
            {
                Console.WriteLine($"Error Code: {exception.ErrorCode}");
                Console.WriteLine($"Message: {exception.Message}");

                return null;
            }
        }
        static void HandleAnalyzeError(TextAnalyticsError error)
        {
            Console.WriteLine($"  Error!");
            Console.WriteLine($"  Action error code: {error.ErrorCode}.");
            Console.WriteLine($"  Message: {error.Message}");
        }
    }
}
