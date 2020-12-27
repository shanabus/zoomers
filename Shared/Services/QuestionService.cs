using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Models;
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

        public async Task AddQuestionAsync(CreateQuestionDto question)
        {
            var q = _mapper.Map<QuestionBase>(question);

            _database.AllQuestions.Add(q);

            await _database.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(EditQuestionDto questionDto)
        {
            var question = _mapper.Map<QuestionBase>(questionDto);

            _database.Entry(question).State = EntityState.Modified;

            await _database.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // todo - check for where used?
            var question = _database.AllQuestions.Find(id);
            
            _database.Remove(question);

            await _database.SaveChangesAsync();
        }
    }
}