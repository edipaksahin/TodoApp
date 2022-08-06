using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Interface;
using TodoApp.Application.Logging;
using TodoApp.Application.Wappers;

namespace TodoApp.Application.Features.Queries.Todo
{
    [IgnoreRequestLogging]
    public class GetAllTask
    {
        public class Query : IRequest<ServiceResponse<List<TaskViewModel>>>{}

        public class Handler : IRequestHandler<Query, ServiceResponse<List<TaskViewModel>>>
        {
            private readonly IApplicationDbContext _applicationDbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext applicationDbContext,
                           IMapper mapper)
            {
                _applicationDbContext = applicationDbContext;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<List<TaskViewModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tasks = await _applicationDbContext.Tasks
                                   .Where(x=> !x.IsDeleted)
                                   .AsNoTracking()
                                   .ToListAsync();

                var response = _mapper.Map<List<TaskViewModel>>(tasks);

                return new ServiceResponse<List<TaskViewModel>>(response);
            }
        }

    }
}
