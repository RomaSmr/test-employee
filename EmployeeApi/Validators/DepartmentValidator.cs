using EmployeeApi.Models;
using FluentValidation;

namespace EmployeeApi.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.Code)
                .NotEmpty().WithMessage("Укажите код")
                .Length(4).WithMessage("Код должен быть ровно из 4 символов");

            RuleFor(d => d.FullName)
                .NotEmpty().WithMessage("Укажите полное наименование");

            RuleFor(d => d.Abbreviation)
                .NotEmpty().WithMessage("Укажите аббревиатуру");
        }
    }
}