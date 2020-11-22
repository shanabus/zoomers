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
        public List<PlayerJoinedPhrase> PlayerJoinedPhrases { get; set; }
        
        public Phrases()
        {
            PlayerJoinedPhrases = GetPlayerJoinedPhrases();
        }            

        public PlayerJoinedPhrase GetRandomPlayerJoinedPhrase(string username, string voiceLang)
        {
            var random = new Random();
            var phrase = PlayerJoinedPhrases[random.Next(PlayerJoinedPhrases.Count)];
            phrase.Text = phrase.Text.Replace("__USER__", username);
            phrase.Lang = voiceLang;

            return phrase;
        }

        public List<PlayerJoinedPhrase> GetPlayerJoinedPhrases() {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvPlayerJoinedPhrasesMapping csvMapper = new CsvPlayerJoinedPhrasesMapping();
            CsvParser<PlayerJoinedPhrase> csvParser = new CsvParser<PlayerJoinedPhrase>(csvParserOptions, csvMapper);
           
            var filePath = ToApplicationPath("Data","player-joined-phrases.csv");
            var phrases = csvParser           
                        .ReadFromFile(filePath, Encoding.ASCII)
                        .Select(x => x.Result)
                        .ToList();
            
            return phrases.Skip(1).ToList(); // miss header row
        }

        private string ToApplicationPath(string path, string fileName)
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exePath, path, fileName);
        }

        public class CsvPlayerJoinedPhrasesMapping : CsvMapping<PlayerJoinedPhrase>
        {
            public CsvPlayerJoinedPhrasesMapping() : base()
            {
                MapProperty(0, x => x.Id);
                MapProperty(1, x => x.Text);
                MapProperty(2, x => x.Pitch);
                MapProperty(3, x => x.Rate);
            }
        }
    }
}