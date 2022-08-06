using AutoMapper;
using FakeItEasy;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Dto.Todo;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.Todo;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class UpdateTaskTests
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly UpdateTask.Command _command;
        private readonly UpdateTask.Handler _handler;
        private readonly IMapper _mapper;

        public UpdateTaskTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _mapper = A.Fake<IMapper>();
            _command = new UpdateTask.Command();
            _handler = new UpdateTask.Handler(_fakeContext, _mapper);
        }

        [Test]
        public void Handle_WhenGivenTodoDoesNotExistInDb_ShouldThrowsEntityNotFoundException()
        {
            _command.Task = Builder<TaskDto>.CreateNew().Build();
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(_command, new CancellationToken()));
        }

        [Test]
        public async Task Handle_WhenGivenTodoExistsInDb_ShouldUpdatesExistingRecord()
        {
            var taskEntity = Builder<Domain.Entities.Task>.CreateNew().Build();
            await _fakeContext.Tasks.AddAsync(taskEntity);
            await _fakeContext.SaveChangesAsync();
            taskEntity.Title = "update";
            A.CallTo(() => _mapper.Map<Domain.Entities.Task>(A<TaskDto>._)).WithAnyArguments().Returns(taskEntity);
            await _handler.Handle(_command, new CancellationToken());
            Assert.IsTrue(_fakeContext.Tasks.FirstOrDefaultAsync(d => d.Id == taskEntity.Id).Result.Title == taskEntity.Title);
        }
    }
}
