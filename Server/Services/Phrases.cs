using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Server.Services
{
    public class Phrases
    {        
        public List<SpokenPhrase> PlayerJoinedPhrases { get; set; }

        public List<SpokenPhrase> AnswersFinishedPhrases { get; set; }
        
        public Phrases()
        {
            PlayerJoinedPhrases = GetPlayerJoinedPhrases();
        }            

        public SpokenPhrase GetRandomPlayerJoinedPhrase(string username, string voiceLang)
        {
            var random = new Random();
            var phrase = PlayerJoinedPhrases[random.Next(PlayerJoinedPhrases.Count)];
            phrase.Text = phrase.Text.Replace("__USER__", username);
            phrase.Lang = voiceLang;

            return phrase;
        }

        public SpokenPhrase GetRandomAnswersFinishedPhrase(string username, string voiceLang)
        {
            var random = new Random();
            var phrase = AnswersFinishedPhrases[random.Next(AnswersFinishedPhrases.Count)];
            phrase.Text = phrase.Text.Replace("__USER__", username);
            phrase.Lang = voiceLang;

            return phrase;
        }

        public List<SpokenPhrase> GetPlayerJoinedPhrases() {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvPhrasesMapping csvMapper = new CsvPhrasesMapping();
            CsvParser<SpokenPhrase> csvParser = new CsvParser<SpokenPhrase>(csvParserOptions, csvMapper);
           
            var filePath = ToApplicationPath("Data","player-joined-phrases.csv");
            var phrases = csvParser           
                        .ReadFromFile(filePath, Encoding.ASCII)
                        .Select(x => x.Result)
                        .ToList();
            
            return phrases.Skip(1).ToList(); // miss header row
        }

        public List<SpokenPhrase> GetAnswersFinishedPhrases() {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvPhrasesMapping csvMapper = new CsvPhrasesMapping();
            CsvParser<SpokenPhrase> csvParser = new CsvParser<SpokenPhrase>(csvParserOptions, csvMapper);
           
            var filePath = ToApplicationPath("Data","answers-finished-phrases.csv");
            var phrases = csvParser           
                        .ReadFromFile(filePath, Encoding.ASCII)
                        .Select(x => x.Result)
                        .ToList();
            
            return phrases.ToList(); // do we need to skip?  it should handle in options??
        }

        private string ToApplicationPath(string path, string fileName)
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exePath, path, fileName);
        }

        public class CsvPhrasesMapping : CsvMapping<SpokenPhrase>
        {
            public CsvPhrasesMapping() : base()
            {
                MapProperty(0, x => x.Id);
                MapProperty(1, x => x.Text);
                MapProperty(2, x => x.Pitch);
                MapProperty(3, x => x.Rate);
            }
        }
    }
}