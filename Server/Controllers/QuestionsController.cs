
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomersClient.Server.Services;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private QuestionService _questionService { get; set; }
        private ILogger<QuestionsController> _logger { get; set; }

        public QuestionsController(QuestionService questionService, ILogger<QuestionsController> logger)
        {
            _questionService = questionService;   
            _logger = logger;
        }

        [HttpGet]
        public List<GameQuestionDto> Get()
        {
            return _questionService.AllQuestions();
        }

        // [HttpGet("fromFile")]
        // public async Task<string> GetFromFile()
        // {
        //     WordPlay wordPlay = new WordPlay();

        //     foreach(var q in wordPlay.Questions)
        //     {
        //         var question = new CreateQuestionDto() {
        //             Question = q.Question,
        //             ImageUrl = "",
        //             Categories = q.CategoriesString
        //         };

        //          await _questionService.AddQuestionAsync(question);

        //          await Task.Delay(500);
        //     }

        //     return wordPlay.Questions.Count + " added to the database";
        // }

        [HttpPost]
        public async Task Post([FromBody] CreateQuestionDto question)
        {
            await _questionService.AddQuestionAsync(question);
        }

        [HttpPut]
        public async Task Put([FromBody] EditQuestionDto question)
        {
            await _questionService.UpdateQuestionAsync(question);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            await _questionService.DeleteAsync(id);
            
            return NoContent();        
        }
    }
}