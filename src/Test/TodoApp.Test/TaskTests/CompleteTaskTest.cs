using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.Todo;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class CompleteTaskTest
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly CompleteTask.Command _command;
        private readonly CompleteTask.Handler _handler;

        public CompleteTaskTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _command = new CompleteTask.Command();
            _handler = new CompleteTask.Handler(_fakeContext);
        }

        [Test]
        public void Handle_WhenGivenTaskDoesNotExistInDb_ShouldThrowsEntityNotFoundException()
        {
            _command.Id = Guid.NewGuid();
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(_command, new CancellationToken()));
        }

        [Test]
        public async Task Handle_WhenGivenTaskExistsInDb_ShouldCompleteExistingRecord()
        {
            var taskEntity = Builder<Domain.Entities.Task>.CreateNew().Build();
            await _fakeContext.Tasks.AddAsync(taskEntity);
            await _fakeContext.SaveChangesAsync();
            _command.Id = taskEntity.Id;
            await _handler.Handle(_command, new CancellationToken());
            Assert.IsTrue(_fakeContext.Tasks.FirstOrDefaultAsync(d => d.Id == taskEntity.Id).Result.IsCompleted == true);
        }
    }
}
