using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Extensions;
using TodoApp.Application.Interface;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Commands.Todo
{
    public class DeleteTask
    {
        public class Command : IRequest<ServiceResponse<bool>>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(i => i.Id)
                    .NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, ServiceResponse<bool>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext applicationDbContext,
                           IMapper mapper)
            {
                _applicationDbContext = applicationDbContext;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var task = await _applicationDbContext
                .Tasks
                .FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);

                if (task.IsNull())
                    throw new EntityNotFoundException("Task", request.Id);

                _applicationDbContext.Tasks.Remove(task);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new ServiceResponse<bool>(true);
            }
        }
    }
}
