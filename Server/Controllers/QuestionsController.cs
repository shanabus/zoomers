
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Shared.Services;

namespace ZoomersClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private QuestionService _questionService { get; set; }

        public QuestionsController(QuestionService questionService)
        {
            _questionService = questionService;   
        }

        [HttpGet]
        public List<GameQuestionDto> Get()
        {
            return _questionService.AllQuestions();
        }
    }
}