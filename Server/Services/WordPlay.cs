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
    public class WordPlay
    {
        public List<WordPlayQuestion> Questions { get; set; }

        public List<WordPlayQuestion> RemainingQuestions => Questions.Where(x => !LastQuestions.Contains(x.Id)).ToList();

        public List<int> LastQuestions { get; set;}

        public WordPlay()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvWordPlayMapping csvMapper = new CsvWordPlayMapping();
            CsvParser<WordPlayQuestion> csvParser = new CsvParser<WordPlayQuestion>(csvParserOptions, csvMapper);
           
            var filePath = ToApplicationPath("Data/Games","charles-game.csv");
            //var filePath = ToApplicationPath("Data/Games","word-play.csv");

            Questions = csvParser           
                        .ReadFromFile(filePath, Encoding.ASCII)
                        .Select(x => x.Result)
                        .ToList();
            
            LastQuestions = new List<int>();
        }

        public WordPlayQuestion GetRandomQuestion(string category)
        {
            var questionPool = string.IsNullOrWhiteSpace(category)? RemainingQuestions : RemainingQuestions.Where(x => x.Categories.Contains(category));

            Random rnd = new Random();
            int question = rnd.Next(questionPool.Count());

            var gameQuestion = questionPool.Skip(question).Take(1).FirstOrDefault();

            // track last question asked, to prevent dups
            if (gameQuestion != null) {
                LastQuestions.Add(gameQuestion.Id);
            }

            return gameQuestion;
        }

        public List<string> GetRemainingCategories()
        {
            return RemainingQuestions.SelectMany(x => x.Categories).Distinct().OrderBy(x => x).ToList();
        }

        private string ToApplicationPath(string path, string fileName)
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exePath, path, fileName);
        }
    }

    public class CsvWordPlayMapping : CsvMapping<WordPlayQuestion>
    {
        public CsvWordPlayMapping()
                : base()
            {
                MapProperty(0, x => x.Id);
                MapProperty(1, x => x.Question);
                MapProperty(2, x => x.CategoriesString);
                MapProperty(3, x => x.ImageUrl);
            }
    }
    
}