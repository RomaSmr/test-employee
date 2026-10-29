using EmployeeApi.Models;
using FluentValidation;

namespace EmployeeApi.Validators
{
    public class ReportFilterValidator : AbstractValidator<ReportFilter>
    {
        public ReportFilterValidator()
        {
            RuleFor(f => f.From)
                .NotNull().WithMessage("Начало периода");

            RuleFor(f => f.To)
                .NotNull().WithMessage("Кконец периода");

            RuleFor(f => f)
                .Must(f => f.From == null || f.To == null || f.From <= f.To)
                .WithMessage("Начало периода позже конца, так нельзя");
        }
    }
}