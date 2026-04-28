/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso.RetailServer.Ecommerce.AbandonedCartSample.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public static class ValidationHelpers
    {
        #region Null & empty checks

        public static void VerifyIsNotNull<T>(
            this T value,
            string name) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void VerifyIsNotNullOrEmpty(this string value, string name)
        {
            VerifyIsNotNull(value, name);

            if (value.Length == 0)
            {
                throw new ArgumentException("Parameter cannot be an empty value", name);
            }
        }

        public static void VerifyIsNotNullOrWhiteSpace(string value, string name)
        {
            VerifyIsNotNull(value, name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Parameter cannot be an empty or whitespace value", name);
            }
        }

        public static void VerifyIsNotNullOrEmpty<T>(ICollection<T> collection, string name)
        {
            VerifyIsNotNull(collection, name);

            if (!collection.Any())
            {
                throw new ArgumentException("Collection must not be empty", name);
            }
        }

        public static void VerifyIsNotNullOrEmpty<T>(IEnumerable<T> collection, string name)
        {
            VerifyIsNotNull(collection, name);

            if (!collection.Any())
            {
                throw new ArgumentException("Enumeration must not be empty", name);
            }
        }

        #endregion

        public static void TryValidate(object @object)
        {
            if (@object == null)
            {
                throw new Exception($"Object of type {@object.GetType().Name} can't be null");
            }

            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                @object, context, results,
                validateAllProperties: true
            );

            if (!isValid)
            {
                throw new Exception($"Object of type {@object.GetType().Name} is not valid. Errors=> {string.Join(":", results.Select(result => result.ErrorMessage))}");
            }
        }
    }

}