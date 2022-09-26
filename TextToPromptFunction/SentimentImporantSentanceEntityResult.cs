using System.Collections.Generic;

namespace TextToPromptFunction
{
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
}
