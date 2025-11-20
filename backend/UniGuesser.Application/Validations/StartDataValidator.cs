using FluentValidation;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.Validations;

public class StartDataValidator : AbstractValidator<StartDataDto>
{
    private readonly int MaximalNicknameLength = 25;
    private readonly int MinimalNicknameLength = 3;


    public StartDataValidator()
    {
        RuleFor(dto => dto.Nickname)
            .MinimumLength(MinimalNicknameLength)
            .WithMessage($"Minimal length of nickname is {MinimalNicknameLength}.")
            .MaximumLength(MaximalNicknameLength)
            .WithMessage($"Maximum length of nickname is {MaximalNicknameLength}.")
            .When(x => !string.IsNullOrEmpty(x.Nickname));
    }
}