using FluentValidation;
using ReLinkApplication.Models;

namespace ReLinkApplication.Validators;

public class UrlValidator : AbstractValidator<Url>
{
    public UrlValidator()
    {
        RuleFor(x => x.ShortUrl).NotEmpty().NotNull();
        //тут надо подумать над моделью самой

        RuleFor(x => x.LongUrl).NotEmpty().NotNull();


    }
}
