using AutoMapper;
using FakeItEasy;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.Todo;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class DeleteTaskTests
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly DeleteTask.Command _command;
        private readonly DeleteTask.Handler _handler;
        private readonly IMapper _mapper;

        public DeleteTaskTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _mapper = A.Fake<IMapper>();
            _command = new DeleteTask.Command();
            _handler = new DeleteTask.Handler(_fakeContext, _mapper);
        }

        [Test]
        public void Handle_WhenGivenIdDoesNotExistInDb_ShouldThrowsEntityNotFoundException()
        {
            _command.Id = Guid.NewGuid();
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(_command, new CancellationToken()));
        }

        [Test]
        public async Task Handle_WhenGivenIdExistsInDb_ShouldDeleteExistingRecord()
        {
            var taskEntity = Builder<Domain.Entities.Task>.CreateNew().Build();
            await _fakeContext.Tasks.AddAsync(taskEntity);
            await _fakeContext.SaveChangesAsync();
            _command.Id = taskEntity.Id;
            await _handler.Handle(_command, new CancellationToken());
            Assert.IsEmpty(_fakeContext.Tasks.Where(d => d.Id == taskEntity.Id && !d.IsDeleted));
        }
    }
}
