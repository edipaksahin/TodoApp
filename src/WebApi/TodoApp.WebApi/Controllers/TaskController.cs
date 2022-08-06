using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Features.Commands.Task;
using TodoApp.Application.Features.Commands.Todo;
using TodoApp.Application.Features.Queries.Todo;
using TodoApp.Application.Wappers;

namespace TodoApp.WebApi.Controllers
{
    [Route("api/task")]
    [ApiController]
    [Authorize]

    public class TaskController: ControllerBase
    {
        private readonly IMediator mediator;

        public TaskController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<List<TaskDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList()
            => Ok(await mediator.Send(new GetAllTask.Query()));

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ServiceResponse<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
            => Ok(await mediator.Send(new GetTask.Query { Id = id }));

        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTodo([FromBody] TaskDto request)
            => Ok(await mediator.Send(new CreateTask.Command { Task = request}));

        [HttpPut]
        [ProducesResponseType(typeof(ServiceResponse<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTodo([FromBody] TaskDto request)
            => Ok(await mediator.Send(new UpdateTask.Command { Task = request }));

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
            => Ok(await mediator.Send(new DeleteTask.Command{ Id = id }));

        [HttpPut("complete/{id}")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompleteTask([FromRoute] Guid id)
            => Ok(await mediator.Send(new CompleteTask.Command { Id = id }));

    }
}
