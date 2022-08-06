using FluentValidation;

namespace TodoApp.Application.Dto.User
{
    public class LoginRequestModel
    {
        public string Email { get; set; }

        public class Validator : AbstractValidator<LoginRequestModel>
        {
            public Validator()
            {
                RuleFor(i => i.Email)
                    .NotNull()
                    .NotEmpty();
            }
        }
    }
}
