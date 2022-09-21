using System;
using System.Collections.Generic;
using System.Linq;

namespace TextToPromptFunction
{
    internal class StableDiffusionDecoration
    {
        private static int maxRandomPick = 3;
        public static List<string> CreateRandomDecoratorsBasedOnSentiment(double sentiment)
        {
            var rnd = new Random();

            //
            // Get the  decorators that are in range of the sentiment
            // 
            var artMedumBySentiment = ParseBySentiment(sentiment, ArtMedium);
            var artistBySentiment = ParseBySentiment(sentiment, Artist);
            var cameraSentiment = ParseBySentiment(sentiment, Camera);
            var filmTypesBySentiment = ParseBySentiment(sentiment, FilmTypes);
            var resolutionBySentiment = ParseBySentiment(sentiment, Resolution);
            var emotionBySentiment = ParseBySentiment(sentiment, Emotion);

            //
            // Takea set of those ((
            var mediums = TakeRandom(rnd, artMedumBySentiment);
            var artists = TakeRandom(rnd, artistBySentiment);
            var cameras = TakeRandom(rnd, cameraSentiment);
            var filmTypes = TakeRandom(rnd, filmTypesBySentiment);
            var resolutions = TakeRandom(rnd, resolutionBySentiment);
            var emotions = TakeRandom(rnd, emotionBySentiment);

            var decorations = mediums.Concat(artists)
                                    .Concat(cameras)
                                    .Concat(filmTypes)
                                    .Concat(resolutions)
                                    .Concat(emotions)
                                    .Concat(cameras)
                                    .ToList();

            return (decorations);
        }

        private static List<string> ParseBySentiment(double sentiment, List<KeyValuePair<double, string>> list)
        {
            return list.Where(s => sentiment >= 0 ? s.Key >= sentiment : s.Key < sentiment).Select(s => s.Value).ToList(); ;
        }

        private static List<string> TakeRandom(Random rnd, List<string> list)
        {
            var maxTake = list.Count > maxRandomPick ? maxRandomPick : list.Count + 1;
            return list.OrderBy(_ => rnd.Next(0, list.Count)).Take(rnd.Next(1, maxTake)).Distinct().ToList();
        }


        public static List<KeyValuePair<double, string>> ArtMedium = new()
        {
                new KeyValuePair<double,string>(1.00, "Illustration"),
                new KeyValuePair<double,string>(.95, "Visual Novel" ),
                new KeyValuePair<double,string>(.92, "Pastel Art" ),
                new KeyValuePair<double,string>(.85, "Graphic Novel" ),
                new KeyValuePair<double,string>(.50, "Oil Paint" ),
                new KeyValuePair<double,string>(.40, "Puppet" ),


                new KeyValuePair<double,string>(.30, "Colored Pencil" ),
                new KeyValuePair<double,string>(.20, "Painting" ),
                new KeyValuePair<double,string>(.01, "Watercolor" ),

                new KeyValuePair<double,string>(-.01, "Drawing" ),
                new KeyValuePair<double,string>(-.12, "Assembly Drawing" ),
                new KeyValuePair<double,string>(-.14, "Crosshatch" ),
                new KeyValuePair<double,string>(-.18, "Penci Art" ),
                new KeyValuePair<double,string>(-.22, "Etching" ),
                new KeyValuePair<double,string>(-.23, "Carving" ),
                new KeyValuePair<double,string>(-.40, "Spray Paint" ),
                new KeyValuePair<double,string>(-.45, "Linocut" ),
                new KeyValuePair<double,string>(-.50, "Street Art" ),
                new KeyValuePair<double,string>(-.99, "Anatomical" )
        };

        public static List<KeyValuePair<double, string>> Artist = new()
        {
                new KeyValuePair<double,string>(1.00, "Andy Warhol"),
                new KeyValuePair<double,string>(.95, "Charlie Bowater" ),
                new KeyValuePair<double,string>(.92, "Claud Monet"),
                new KeyValuePair<double,string>(.90, "Lemma Guya" ),
                new KeyValuePair<double,string>(.83, "WLOP" ),
                new KeyValuePair<double,string>(.20, "Charlie Bowater" ),
                new KeyValuePair<double,string>(.19, "Albert Bierstadt" ),
                new KeyValuePair<double,string>(.10, "Vincent van Gogh" ),
                new KeyValuePair<double,string>(.01, "Bob Ross" ),

                new KeyValuePair<double,string>(-.01, "Salvador Dali" ),
                new KeyValuePair<double,string>(-.20, "Yoshitaka Amano" ),
                new KeyValuePair<double,string>(-.22, "Zeng Fanzhi" ),
                new KeyValuePair<double,string>(-.25, "Ralph Steadman" ),
                new KeyValuePair<double,string>(-.27, "John Kenn Mortensen" ),
                new KeyValuePair<double,string>(-.39, "Hieronymus Bosch" ),
                new KeyValuePair<double,string>(-.40, "Pieter Bruegel The Elder" ),
                new KeyValuePair<double,string>(-.52, "Kiki Smith" ),
                new KeyValuePair<double,string>(-.55, "H.R. Giger" ),
                new KeyValuePair<double,string>(-.99, "Assassin's Creed" )
        };
        public static List<KeyValuePair<double, string>> Camera = new()
        {
                new KeyValuePair<double,string>(1.0, "Photography"),
                new KeyValuePair<double,string>(.29, "Photoshoot"),
                new KeyValuePair<double,string>(.01, "Portait"),
                new KeyValuePair<double,string>(-.01, "Film Grain" ),
                new KeyValuePair<double,string>(-.99, "War Photography" ),
        };

        public static List<KeyValuePair<double, string>> FilmTypes = new()
        {
                new KeyValuePair<double,string>(1.0, "Nikon D750"),
                new KeyValuePair<double,string>(.5, "Photoshoot"),
                new KeyValuePair<double,string>(.01, "Portait"),
                new KeyValuePair<double,string>(-.01, "Daguerrotype" ),
                new KeyValuePair<double,string>(-.99, "Ambrotype" )
        };
        public static List<KeyValuePair<double, string>> Resolution = new()
        {
                new KeyValuePair<double,string>(1.0, "4k"),
                new KeyValuePair<double,string>(.5, "Ultra-HD"),
                new KeyValuePair<double,string>(.01, "32k"),

                new KeyValuePair<double,string>(-.01, "HD" ),
                new KeyValuePair<double,string>(-.99, "HD" )
        };

        public static List<KeyValuePair<double, string>> Emotion = new()
        {
                new KeyValuePair<double,string>(1.0, "Happy"),
                new KeyValuePair<double,string>(.90, "Excited"),
                new KeyValuePair<double,string>(.01, "Angelic"),

                new KeyValuePair<double,string>(-.01, "Sad"),
                new KeyValuePair<double,string>(-.5, "Angry"),
                new KeyValuePair<double,string>(-.99, "Evil")
        };
    }
}

