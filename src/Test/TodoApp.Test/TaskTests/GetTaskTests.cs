using AutoMapper;
using FakeItEasy;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Queries.Todo;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class GetTaskTests
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly GetTask.Query _query;
        private readonly GetTask.Handler _handler;
        private readonly IMapper _mapper;


        public GetTaskTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _mapper = A.Fake<IMapper>();
            _query = new GetTask.Query();
            _handler = new GetTask.Handler(_fakeContext, _mapper);
        }

        [Test]
        public void Handle_WhenGivenIdDoesNotExistInDb_ShouldThrowsEntityNotFoundException()
        {
            _query.Id = Guid.NewGuid();
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await _handler.Handle(_query, new CancellationToken()));
        }

        [Test]
        public async Task Handle_WhenGivenIdExistsInDb_ShouldGetsRecord()
        {
            var taskEntity = Builder<Domain.Entities.Task>.CreateNew().Build();
            await _fakeContext.Tasks.AddAsync(taskEntity);
            await _fakeContext.SaveChangesAsync();
            _query.Id = taskEntity.Id;
            var foundedEntity = await _handler.Handle(_query, new CancellationToken());
            Assert.IsNotNull(foundedEntity);
        }
    }
}
