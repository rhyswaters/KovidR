using System;
using AutoMapper;
using EventBus.Messages.Events;
using Guess.Application.Features.CaseNumbers.Commands.CreateCaseNumbers;
using Guess.Domain.Entities;

namespace Guess.API.Mapping
{
    public class GuessProfile : Profile
    {
        public GuessProfile()
        {
            CreateMap<CreateCaseNumbersCommand, CaseNumbersPublishedEvent>().ReverseMap();
            CreateMap<CreateCaseNumbersCommand, CaseNumbers>().ReverseMap();
        }
    }
}
