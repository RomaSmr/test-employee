using EmployeeApi.Models;
using FluentValidation;

namespace EmployeeApi.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.TabNumber)
                .NotEmpty().WithMessage("Укажите табельный номер");

            RuleFor(e => e.LastName)
                .NotEmpty().WithMessage("Укажите фамилию");

            RuleFor(e => e.FirstName)
                .NotEmpty().WithMessage("Укажите имя");

            RuleFor(e => e.MiddleName)
                .NotEmpty().WithMessage("Укажите отчество");
        }
    }
}