using FluentValidation;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Validators
{
    public class ProjectModelValidator : AbstractValidator<ProjectModel>
    {
        public ProjectModelValidator()
        {
            RuleFor(x => x.Name).Length(3, 20);
        }
    }
}