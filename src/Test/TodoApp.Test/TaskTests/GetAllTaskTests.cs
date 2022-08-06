using AutoMapper;
using FakeItEasy;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Features.Queries.Todo;
using TodoApp.Application.Interface;
using TodoApp.Persistence.Context;

namespace TodoApp.Test.TaskTests
{
    public class GetAllTaskTests
    {
        private readonly IApplicationDbContext _fakeContext;
        private readonly GetAllTask.Query _query;
        private readonly GetAllTask.Handler _handler;
        private readonly IMapper _mapper;
        private Guid _id;


        public GetAllTaskTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllTodoHandlerTest")
                .Options;
            _fakeContext = new ApplicationDbContext(options);
            _mapper = A.Fake<IMapper>();
            _query = new GetAllTask.Query();
            _handler = new GetAllTask.Handler(_fakeContext, _mapper);
        }

        [SetUp]
        public void SetUp()
        {
            _fakeContext.Tasks.RemoveRange(_fakeContext.Tasks);
            _fakeContext.SaveChangesAsync();
        }

        [Test]
        public async Task Handle_WhenAnyRecordExistInDb_ShouldReturnEmptyList()
        {
            var foundedEntity = await _handler.Handle(_query, new CancellationToken());
            Assert.IsEmpty(foundedEntity.Data);
        }

        [Test]
        public async Task Handle_WhenAlways_ShouldGetsAllRecords()
        {
            var taskEntities = Builder<Domain.Entities.Task>
                .CreateListOfSize(20).Random(5).With(entity => entity.Id = _id).Build();
            await _fakeContext.Tasks.AddRangeAsync(taskEntities);
            await _fakeContext.SaveChangesAsync();
            var allEntities = await _handler.Handle(_query, new CancellationToken());
            Assert.IsNotNull(allEntities);
            Assert.IsTrue(allEntities.Data.All(blocks => blocks.Id == _id));
        }
    }
}
