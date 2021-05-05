using System;
using AutoMapper;
using Guess.Application.Features.Guesses.Commands.CreateGuess;
using Guess.Application.Features.Results.GetResultsByDaysWon.Queries;
using Guess.Domain.Entities;

namespace Guess.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserGuess, CreateGuessCommand>().ReverseMap();
        }
    }
}
