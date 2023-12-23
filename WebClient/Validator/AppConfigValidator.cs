using Contract.AppConfigs;
using Core.Const;
using FluentValidation;
using WebClient.LanguageResources;

namespace WebClient.Validator
{
    public class AppConfigValidator : AbstractValidator<CreateUpdateAppConfigDto>
    {
        public AppConfigValidator(JsonStringLocalizer localizer)
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
            RuleFor(x => x.CompanyName).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
            
            When(x => x.EnableNotificationByEmail, () =>
            {
                RuleFor(x => x.MailHost).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.MailPort).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.MailName).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.MailName).Matches(ContentRegularExpression.EMAIL)
                    .WithMessage(localizer["Validator.MatchEmail"]);

                RuleFor(x => x.Password).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
            });
            
            When(x => x.Firebase, () =>
            {
                RuleFor(x => x.APIKey).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.AuthDomain).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.MessagingSenderId).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.APPID).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
                RuleFor(x => x.ServerKey).NotEmpty().WithMessage(localizer["Validator.IsRequired"]);
            });
        }
    }
}