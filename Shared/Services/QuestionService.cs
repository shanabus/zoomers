using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Models.DTOs;

namespace ZoomersClient.Shared.Services
{
    public class QuestionService
    {
        private ILogger<QuestionService> _logger { get; set; }
        private ApplicationDBContext _database { get; set; }
        private readonly IMapper _mapper;

        public QuestionService(ILogger<QuestionService> logger, ApplicationDBContext database, IMapper mapper)
        {
            _logger = logger;
            _database = database;
            _mapper = mapper;
        }

        public List<GameQuestionDto> AllQuestions()
        {
            var questions = _database.AllQuestions.ToList();

            return _mapper.Map<List<GameQuestionDto>>(questions);
        }
    }
}