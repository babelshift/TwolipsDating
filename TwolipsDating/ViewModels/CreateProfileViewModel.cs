using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;

namespace TwolipsDating.ViewModels
{
    public class CreateProfileViewModel
    {
        public bool IsCurrentUserEmailConfirmed { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int? BirthMonth { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
        public int? BirthDayOfMonth { get; set; }

        [Required]
        [Birthday("BirthDayOfMonth", "BirthMonth", "BirthYear", ErrorMessage = "You must be at least 18 years old.")]
        [RangeYearTo18YearsAgo(1900, ErrorMessage="You must be at least 18 years old.")]
        public int? BirthYear { get; set; }

        [Required]
        public int? SelectedGenderId { get; set; }

        [Required(ErrorMessage = "You must select a location from the drop down menu")]
        public string SelectedLocation { get; set; }

        public IDictionary<int, string> Genders { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RangeYearTo18YearsAgo : RangeAttribute, IClientValidatable
    {
        public RangeYearTo18YearsAgo(int from) : base(from, DateTime.Today.AddYears(-18).Year) { }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rules = new ModelClientValidationRangeRule(this.ErrorMessage, this.Minimum, this.Maximum);
            yield return rules;
        }

        #endregion
    }

    public class BirthdayAttribute : ValidationAttribute, IClientValidatable
    {
        private string birthDayField;
        private string birthMonthField;
        private string birthYearField;

        public BirthdayAttribute(string birthDayField, string birthMonthField, string birthYearField)
        {
            this.birthDayField = birthDayField;
            this.birthMonthField = birthMonthField;
            this.birthYearField = birthYearField;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int? birthDay = (int?)validationContext.ObjectType.GetProperty(birthDayField).GetValue(validationContext.ObjectInstance, null);
            int? birthMonth = (int?)validationContext.ObjectType.GetProperty(birthMonthField).GetValue(validationContext.ObjectInstance, null);
            int? birthYear = (int?)validationContext.ObjectType.GetProperty(birthYearField).GetValue(validationContext.ObjectInstance, null);

            if (birthDay.HasValue && birthMonth.HasValue && birthYear.HasValue)
            {
                DateTime enteredBirthdate = new DateTime(birthYear.Value, birthMonth.Value, birthDay.Value);
                DateTime dateTime18YearsAgo = DateTime.Today.AddYears(-18);
                if (enteredBirthdate > dateTime18YearsAgo)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }
            }
            else
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "multifield"
            };
        }
    }
}