using UniGuesser.Application.ValueObjects;

namespace UniGuesser.Application.Models.GameModels
{
    public class AnswerDto
    {
        public Coordinates Coordinates { get; set; }
        public string? Token { get; set; }
    }
}
