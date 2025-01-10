using FluentValidation;
using PartyGame.Models;

public class StartDataValidator : AbstractValidator<StartDataDto>
{
    private readonly int MinimalNicknameLength = 3;
    private readonly int MaximalNicknameLength = 25;
    private readonly string[] difficultyLevels = new string[] { "easy", "normal", "hard" };


    public StartDataValidator()
    {
        RuleFor(dto => dto.Difficulty)
            .NotEmpty()
            .WithMessage("Difficulty cannot be empty.")
            .Must(difficulty => difficultyLevels.Contains(difficulty.ToLower()))  
            .WithMessage("Invalid difficulty level. Valid options are: easy, normal, hard.");

        RuleFor(dto => dto.Nickname)
            .NotEmpty()
            .WithMessage("Nickname cannot be empty.")
            .MinimumLength(MinimalNicknameLength)
            .WithMessage($"Minimal length of nickname is {MinimalNicknameLength}.")
            .MaximumLength(MaximalNicknameLength)
            .WithMessage($"Maximum length of nickname is {MaximalNicknameLength}.");
    }
}