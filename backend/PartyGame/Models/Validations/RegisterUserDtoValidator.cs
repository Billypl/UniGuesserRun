using FluentValidation;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Repositories;

namespace PartyGame.Models.Validations
{
    public class RegisterUserDtoValidator:AbstractValidator<RegisterUserDto>
    {
        private readonly IAccountRepository _accountRepository;
        public RegisterUserDtoValidator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .Custom((value, context) =>
                {
                    if (_accountRepository.GetUserByNicknameOrEmailAsync(value).Result is not null)
                    {
                        context.AddFailure(value,"Email is taken");
                    }
                });

            RuleFor(u => u.Nickname)
                .NotEmpty()
                .MinimumLength(3)
                .Custom((value, context) =>
                {
                   
                });

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("Minimum length of password is 8");

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password)
                .WithMessage("Passwords are not the same ");
        }
    }
}
