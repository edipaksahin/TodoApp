using FluentValidation;
using System;

namespace TodoApp.Application.Dto.Todo
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public class Validator : AbstractValidator<TaskDto>
        {
            public Validator()
            {
                RuleFor(i => i.Title)
                    .NotEmpty()
                    .NotEmpty();

                RuleFor(i => i.Description)
                    .NotEmpty()
                    .MaximumLength(200);
            }
        }
    }
}
