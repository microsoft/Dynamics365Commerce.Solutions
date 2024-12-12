/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.AtolSample
    {
        using System;
        using System.Linq;
        using System.Runtime.Serialization;

        /// <summary>
        /// Represents an enumeration serializer by <see cref="EnumMemberAttribute"/>.
        /// </summary>
        public static class EnumMemberSerializer
        {
            /// <summary>
            /// Serializes an enumeration by the <see cref="EnumMemberAttribute"/>.
            /// </summary>
            /// <typeparam name="TEnum">Enumeration type.</typeparam>
            /// <param name="value">Serializable value.</param>
            /// <returns>If EnumMember is set, it returns its value, otherwise it returns the field name.</returns>
            public static string Serialize<TEnum>(TEnum value)
            {
                var attribute = typeof(TEnum)
                    .GetMember(value.ToString())
                    .SingleOrDefault()
                    ?.GetCustomAttributes(false)
                    ?.OfType<EnumMemberAttribute>().SingleOrDefault();
                return attribute == null ? value.ToString() : attribute.Value;
            }

            /// <summary>
            /// Deserializes an enumeration by the <see cref="EnumMemberAttribute"/>.
            /// </summary>
            /// <typeparam name="TEnum">Enumeration type.</typeparam>
            /// <param name="value">Deserialized value.</param>
            /// <returns>Returns the enumeration value.</returns>
            public static TEnum Deserialize<TEnum>(string value)
            {
                var fieldName = typeof(TEnum)
                        .GetFields()
                        .Where(field => field.GetCustomAttributes(false).OfType<EnumMemberAttribute>().SingleOrDefault()?.Value.Equals(value) ?? false)
                        .Single().Name;
                return (TEnum)Enum.Parse(typeof(TEnum), fieldName);
            }
        }
    }
}
