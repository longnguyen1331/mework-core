using Contract.Identity.RoleManager;
using FluentValidation;
using WebClient.LanguageResources;

namespace WebClient.Validator
{
    public class RoleValidator : AbstractValidator<CreateUpdateRoleDto>
    {
        
        public RoleValidator(JsonStringLocalizer localizer)
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage(localizer["Hello"]);
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer["Hello"]);
        }
    }
}