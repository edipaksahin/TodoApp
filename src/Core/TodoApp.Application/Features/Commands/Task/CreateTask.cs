using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Interface;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Commands.Task
{
    public class CreateTask
    {
        public class Command : IRequest<ServiceResponse<Guid>>
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

        public class Handler : IRequestHandler<Command, ServiceResponse<Guid>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext applicationDbContext, 
                           IMapper mapper)
            {
                _applicationDbContext = applicationDbContext;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var task = _mapper.Map<Domain.Entities.Task>(request.Task);

                await _applicationDbContext.Tasks.AddAsync(task);
                await _applicationDbContext.SaveChangesAsync();

                return new ServiceResponse<Guid>(task.Id);
            }
        }
    }
}
