using System;
using AutoMapper;
using Guess.Application.Features.Guesses.Commands;
using Guess.Application.Features.Results.Queries;
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
