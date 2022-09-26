using System;
using System.Collections.Generic;
using System.Linq;
using static TextToPromptFunction.StableDiffusionDecoration;

namespace TextToPromptFunction
{
    public class StableDiffusionDecoration
    {
        private static readonly Random rnd = new();

        public static List<string> CreateRandomDecoratorsBasedOnSentiment(double postive, double negative, double neutral, int maxReturned = 2)
        {

            //
            // Get the  decorators that are in range of the sentiment
            // 

            var artMediumBySentiment = ParseSentiments(postive, negative, neutral, ArtMedium, 3);
            var artistBySentiment = ParseSentiments(postive, negative, neutral, Artist, 2);
            var filmTypesBySentiment = ParseSentiments(postive, negative, neutral, FilmTypes, 3);
            var resolutionBySentiment = ParseSentiments(postive, negative, neutral, Resolution, 3);
            var emotionBySentiment = ParseSentiments(postive, negative, neutral, Emotion, 10);
            var cameraSentiment = ParseSentiments(postive, negative, neutral, Camera, 3);

            //
            // Takea set of those ((
            //var mediums = TakeRandom(rnd, artMedumBySentiment);
            //var artists = TakeRandom(rnd, artistBySentiment);
            //var cameras = TakeRandom(rnd, cameraSentiment);
            //var filmTypes = TakeRandom(rnd, filmTypesBySentiment);
            //var resolutions = TakeRandom(rnd, resolutionBySentiment);
            //var emotions = TakeRandom(rnd, emotionBySentiment);

            var artistBySentimentBy = new List<string>() { "in the syle of " + string.Join(" and ", artistBySentiment) };
            var decorations = emotionBySentiment
                                    .Concat(artistBySentimentBy)
                                    .Concat(cameraSentiment)
                                    .Concat(filmTypesBySentiment)
                                    .Concat(resolutionBySentiment)
                                    .Concat(artMediumBySentiment)
                                    .ToList();

            return (decorations);
        }

        private static int PickMaxDecorationCount(double sentiment)
        {
            return sentiment switch
            {
                < 0.25 => 0,
                < 0.50 => 1,
                < 0.75 => 3,
                _ => 4,
            };
        }

        private static List<string> ParseSentiments(double positve, double negative, double neutral, List<KeyValuePair<Sentiment, string>> list, int maxPick = 10)
        {
            var pos = ParseBySentiment(Sentiment.Postive, positve, list);
            var neg = ParseBySentiment(Sentiment.Negative, negative, list);
            var neu = ParseBySentiment(Sentiment.Neutral, neutral, list);

            return pos.Concat(neg)
                    .Concat(neu)
                    .Take(maxPick)
                    .ToList();
        }

        private static List<string> ParseBySentiment(Sentiment sentimentType, double sentiment, List<KeyValuePair<Sentiment, string>> list)
        {
            var maxRandomPick = PickMaxDecorationCount(sentiment);

            if (maxRandomPick == 0)
            {
                return new List<string>();
            }

            var sentiments = list.Where(s => s.Key == sentimentType).Select(s => s.Value).ToList();

            var trimmedList = TakeRandom(rnd, sentiments, maxRandomPick);

            return (trimmedList);
        }

        private static List<string> TakeRandom(Random rnd, List<string> list, int maxRandomPick)
        {
            var maxTake = list.Count > maxRandomPick ? maxRandomPick : list.Count + 1;
            return list.OrderBy(_ => rnd.Next(0, list.Count)).Take(rnd.Next(1, maxTake)).Distinct().ToList();
        }


        public enum Sentiment
        {
            Postive,
            Negative,
            Neutral
        }

        public static List<KeyValuePair<Sentiment, string>> ArtMedium = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Illustration"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Visual Novel" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Pastel Art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Graphic Novel" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Oil Paint" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Puppet" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Done in LEGO" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "German romanticism" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Puppet" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "impressionism" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "the matrix" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Anime" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Colored Pencil" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Painting" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Watercolor" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "American Romanticism" ),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Drawing" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Assembly Drawing" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Crayon art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Blueprint" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "60s kitsch and psychedelia" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Digital Art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Gothic Art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Graffiti" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Retrowave" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Street art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Avant-garde" ),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Chillwave" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Dark Academia" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Crosshatch" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Pencil Art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Etching" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Carving" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Spray Paint" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Linocut" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Street Art" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Vaporwave" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Anatomical" )
        };

        public static List<KeyValuePair<Sentiment, string>> Artist = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Andy Warhol"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Charlie Bowater" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Claude Monet"),                
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "WLOP" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Leonardo da Vinci" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Rembrandt" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Sandro Botticelli" ),               
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "David Hockney" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Bob Byerley" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Kim Tschang Yeul" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "George Lucas" ),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Charlie Bowater" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Albert Bierstadt" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Vincent van Gogh" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Bob Ross" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Ray Caesar" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Rene Margritte" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Marc Chagall" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Salvador Dali" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Mark Ryden" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Yoshitaka Amano" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Camille Corot" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Ernst Ludwig Kirchner" ),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Zeng Fanzhi" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Weta Digital" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Aubrey Beardsley" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Pablo Picasso" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Frank Frazetta" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Ralph Steadman" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "John Kenn Mortensen" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Edvard Munch" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Hieronymus Bosch" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Pieter Bruegel The Elder" ),                
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "H.R. Giger" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Edward Hopper" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Assassin's Creed" )
        };

        public static List<KeyValuePair<Sentiment, string>> Camera = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Photography"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Photoshoot"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Portrait"),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Long Exposure"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "F/22"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "F/2.8"),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Film Grain" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Double Exposure" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "War Photography")
        };

        public static List<KeyValuePair<Sentiment, string>> FilmTypes = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Nikon D750"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Photoshoot"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Hyperspectral "),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "35mm"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "DSLR"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Ektachrome"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Lomo"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Provia"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Shot on 70mm"),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Portrait"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Instax" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Vintage" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Kodak Portra" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Schlieren" ),                

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Daguerreotype" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Caoltype" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Tintype" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Tri-X 400 TX" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Pinhole Photography" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Night Vision"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Ambrotype" )
        };

        public static List<KeyValuePair<Sentiment, string>> Resolution = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "4k"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "8K"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "32k"),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Ultra-HD"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "32k"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Megapixel"),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "HD" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Full-HD" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Ultra-HD" )
        };

        public static List<KeyValuePair<Sentiment, string>> Misc = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "intricate details"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "highly detailed"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "32k"),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Ultra-HD"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "32k"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Megapixel"),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "HD" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Full-HD" ),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Ultra-HD" )

        };


        public static List<KeyValuePair<Sentiment, string>> Emotion = new()
        {
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Happy"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Excited"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Angelic"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Smiling"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Grinning"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Love"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Inspiration"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Fun"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Pleasure"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Joy"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Love"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Delighted"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Astonished"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Amusement"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Serenity"),
                new KeyValuePair<Sentiment,string>(Sentiment.Postive, "Awe"),

                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Bored"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Pride"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Surprise"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Distracted"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Realization"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Confusion"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Content"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Calm"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Trust"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Acceptance"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Relaxed"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Anticipation"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Interest"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Obstinate"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Polite"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Puzzled"),
                new KeyValuePair<Sentiment,string>(Sentiment.Neutral, "Restless"),

                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Sad"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Frown"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Scowl"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Angry"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Disgust"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Fear"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Pain"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Sadness"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Lonely"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Miserable"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Gloomy"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Offended"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Alarmed"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Afraid"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Dislike"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Guilt"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Regret"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Embarrassment"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Jealousy"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Evil"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Outrage"),
                new KeyValuePair<Sentiment,string>(Sentiment.Negative, "Shame")
        };
    }
}
