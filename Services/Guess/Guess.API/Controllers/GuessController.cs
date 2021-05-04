using System;
using System.Net;
using System.Threading.Tasks;
using Guess.Application.Features.Guesses.Commands;
using Guess.Application.Features.Results.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Guess.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GuessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GuessController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> CreateGuess([FromBody] CreateGuessCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet(Name = "GetResultsByDaysWon")]
        [ProducesResponseType(typeof(ResultsByDaysWonVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ResultsByDaysWonVm>> GetResultsByDaysWon(DateTime? from, DateTime? to)
        {
            var query = new GetResultsByDaysWonQuery(from, to);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
    }
}
