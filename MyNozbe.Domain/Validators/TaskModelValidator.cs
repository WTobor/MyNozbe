using FluentValidation;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain.Validators
{
    public class TaskModelValidator : AbstractValidator<TaskModel>
    {
        public TaskModelValidator()
        {
            RuleFor(x => x.Name).Length(3, 30);
        }
    }
}