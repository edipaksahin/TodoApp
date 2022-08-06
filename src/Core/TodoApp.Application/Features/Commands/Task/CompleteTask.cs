using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Extensions;
using TodoApp.Application.Interface;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Commands.Todo
{
    public class CompleteTask
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

            public Handler(IApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<ServiceResponse<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var task = await _applicationDbContext
                .Tasks
                .FindAsync(new object[] { request.Id}, cancellationToken: cancellationToken);

                if (task.IsNull())
                    throw new EntityNotFoundException("Task", request.Id);

                task.SetCompleted();

                await _applicationDbContext.SaveChangesAsync();

                return new ServiceResponse<bool>(true);
            }
        }
    }
}
