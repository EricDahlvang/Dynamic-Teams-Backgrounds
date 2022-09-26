namespace TextToPromptFunction
{
    public class TextToPromptInput
    {
        public enum PromptContentTypes
        {
            rawText,
            Entities
        }

        string text;
        double minConfidenceScore;
        private PromptContentTypes promptContentType; 

        public string Text { get => text; set => text = value; }
        public double MinConfidenceScore { get => minConfidenceScore; set => minConfidenceScore = value; }
        public PromptContentTypes PromptContentType { get => promptContentType; set => promptContentType = value; }
    }
}
