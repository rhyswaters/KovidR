using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Guess.Application.Features.Guesses.Commands.CreateGuess;
using Guess.Application.Features.Results.GetResultsByCumulativeTotals.Queries;
using Guess.Application.Features.Results.GetResultsByDaysWon.Queries;
using Guess.Application.Features.Results.Queries.GetResultsList;
using Guess.Application.Features.Guesses.Queries.HasUserSubmittedNextGuess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Guess.API.Controllers
{
    [EnableCors("default")]
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
        //[Authorize("write:guesses")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> CreateGuess([FromBody] CreateGuessCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
        
        [HttpGet("GetResultsByDaysWon")]
        //[Authorize("read:guesses")]
        [ProducesResponseType(typeof(ResultsByDaysWonVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ResultsByDaysWonVm>> GetResultsByDaysWon(DateTime? from, DateTime? to)
        {
            var query = new GetResultsByDaysWonQuery(from, to);
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpGet("GetResultsByCumulativeTotals")]
        //[Authorize("read:guesses")]
        [ProducesResponseType(typeof(ResultsByDaysWonVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ResultsByDaysWonVm>> GetResultsByCumulativeTotals(DateTime? from, DateTime? to)
        {
            var query = new GetResultsByCumulativeTotalsQuery(from, to);
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpGet("GetResultsList/{numResultsToFetch}")]
        //[Authorize("read:guesses")]
        [ProducesResponseType(typeof(IEnumerable<ResultsListVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ResultsListVm>>> GetResultsList(int numResultsToFetch)
        {
            var query = new GetResultsListQuery(numResultsToFetch);
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpGet("HasUserSubmittedNextGuess/{userName}")]
        //[Authorize("read:guesses")]
        [ProducesResponseType(typeof(HasUserSubmittedNextGuessVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<HasUserSubmittedNextGuessVm>> HasUserSubmittedNextGuess(string userName)
        {
            var query = new HasUserSubmittedNextGuessQuery(userName);
            var results = await _mediator.Send(query);
            return Ok(results);
        }
    }
}
