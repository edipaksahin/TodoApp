using AutoMapper;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Extensions;
using TodoApp.Application.Interface;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Commands.Todo
{
    public class UpdateTask
    {
        public class Command : IRequest<ServiceResponse<TaskDto>>
        {
            public TaskDto Task { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(i => i.Task)
                    .NotNull()
                    .SetValidator(new TaskDto.Validator());
            }
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<TaskDto>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext applicationDbContext,
                           IMapper mapper)
            {
                _applicationDbContext = applicationDbContext;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<TaskDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var task = await _applicationDbContext
                .Tasks
                .FindAsync(new object[] { request.Task.Id}, cancellationToken: cancellationToken);

                if (task.IsNull())
                    throw new EntityNotFoundException("Task", request.Task.Id);

                _mapper.Map(request.Task, task);

                await _applicationDbContext.SaveChangesAsync();

                var response = _mapper.Map<TaskDto>(task);

                return new ServiceResponse<TaskDto>(response);
            }
        }
    }
}
