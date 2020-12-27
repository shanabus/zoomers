using System;
using AutoMapper;
using ZoomersClient.Shared.Models;
using ZoomersClient.Shared.Models.DTOs;

namespace ZoomersClient.Server
{
    public class AutoMapperProfile : Profile  
    {  
        public AutoMapperProfile()  
        {  
            CreateMap<Player, PlayerDto>();  
            CreateMap<PlayerDto, Player>();  
            CreateMap<GameQuestion, GameQuestionDto>();  
            CreateMap<AnsweredQuestion, AnsweredQuestionDto>();
            CreateMap<AudienceScore, AudienceScoreDto>();
            CreateMap<Game, GameDto>();  

            CreateMap<QuestionBase, GameQuestionDto>();
        }  
    }  
}