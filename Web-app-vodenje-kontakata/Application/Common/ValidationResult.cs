namespace Application.Common
{
    public class ValidationResult
    {
        public List<string> ValidationItems { get; set; } = new();
        public bool IsSuccess => !ValidationItems.Any();
    }
}
