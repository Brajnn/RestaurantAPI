using FluentValidation;
using RestaurantAPI.Entities;
using System.Linq;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator:AbstractValidator<RestaurantQuery>
    {
        private string[] allowedSortByColumnNames = { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description) };
        private int[] allowedPageSizes = new[] { 5, 10, 15 }; 
        public RestaurantQueryValidator()
        {
            RuleFor(r=> r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }
            });
            RuleFor(r=>r.SortBy).Must(value=> string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort ny is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }
    }
}
