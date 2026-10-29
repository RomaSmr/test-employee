using EmployeeApi.Models;
using FluentValidation;

namespace EmployeeApi.Validators
{
    public class TransferValidator : AbstractValidator<TransferRequest>
    {
        public TransferValidator()
        {
            RuleFor(t => t.DepartmentId)
                .GreaterThan(0).WithMessage("Выберите подразделение");

            RuleFor(t => t.Position)
                .NotEmpty().WithMessage("Какая должность");
        }
    }
}