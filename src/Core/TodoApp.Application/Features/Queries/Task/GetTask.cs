using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Extensions;
using TodoApp.Application.Interface;
using TodoApp.Application.Logging;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Queries.Todo
{
    [IgnoreRequestLogging]
    public class GetTask
    {
        public class Query : IRequest<ServiceResponse<TaskViewModel>> 
        {
            public Guid Id { get; set; }
        }
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(i => i.Id)
                    .NotNull();
            }
        }

        public class Handler : IRequestHandler<Query, ServiceResponse<TaskViewModel>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly IMapper _mapper;


            public Handler(IApplicationDbContext applicationDbContext,
                           IMapper mapper)
            {
                _applicationDbContext = applicationDbContext;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<TaskViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var task = await _applicationDbContext
                                 .Tasks
                                 .AsNoTracking()
                                 .Where(x=> x.Id == request.Id && !x.IsDeleted)
                                 .FirstOrDefaultAsync();

                if (task.IsNull())
                    throw new EntityNotFoundException("Task", request.Id);

                var response = _mapper.Map<TaskViewModel>(task);

                return new ServiceResponse<TaskViewModel>(response);
            }
        }

    }
}
