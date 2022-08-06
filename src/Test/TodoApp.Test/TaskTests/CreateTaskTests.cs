using AutoMapper;
using FakeItEasy;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Features.Commands.Task;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class CreateTaskTests
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly CreateTask.Command _command;
        private readonly CreateTask.Handler _handler;
        private readonly IMapper _mapper;

        public CreateTaskTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _mapper = A.Fake<IMapper>();
            _command = new CreateTask.Command();
            _handler = new CreateTask.Handler(_fakeContext, _mapper);
        }

        [Test]
        public async Task Handle_WhenAlways_ShouldCreateANewTodo()
        {
            var taskEntity = Builder<Domain.Entities.Task>.CreateNew().Build();
            A.CallTo(() => _mapper.Map<Domain.Entities.Task>(A<TaskDto>._)).WithAnyArguments().Returns(taskEntity);
            await _handler.Handle(_command, new CancellationToken());
            Assert.IsNotEmpty(_fakeContext.Tasks.Where(d => d.Id == taskEntity.Id));
        }
    }
}
