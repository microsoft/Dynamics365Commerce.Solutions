namespace Contoso
{
    namespace Commerce.Runtime.DocumentProvider.PosnetSample.PosnetProtocol
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Reflection;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel;
        using Contoso.Commerce.Runtime.DocumentProvider.PosnetSample.DocumentModel.Protocol;
        using Microsoft.Dynamics.Commerce.Runtime;
        using RequiredDataAnnotationsAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

        /// <summary>
        /// Creates a POSNET printer request.
        /// </summary>
        internal static class PrinterRequestBuilder
        {
            private static HashSet<Type> _supportedCLRTypes = new HashSet<Type> { typeof(int), typeof(long), typeof(decimal), typeof(bool), typeof(DateTime), typeof(string) };

            /// <summary>
            /// Builds a POSNET command from object marked with CommandAttribute and CommandParameterAttribute.
            /// </summary>
            /// <param name="attributedObject">An object.</param>
            /// <returns>Posnet command.</returns>
            public static IPosnetCommandRequest BuildRequestCommand(object attributedObject)
            {
                var t = attributedObject.GetType();
                CommandAttribute commandNameAttribute = Attribute.GetCustomAttribute(t, typeof(CommandAttribute)) as CommandAttribute;

                if (commandNameAttribute == null)
                {
                    throw new NotSupportedException(string.Format($"Entity of type '{t}' is not supported as a command."));
                }

                List<CommandParameter> parameters = new List<CommandParameter>();
                Dictionary<string, DataType> results = new Dictionary<string, DataType>();

                var props = t.GetProperties(BindingFlags.GetProperty |
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var attributes = Attribute.GetCustomAttributes(prop, true);

                    CommandParameterAttribute commandParmAttr = attributes.FirstOrDefault(x => x is CommandParameterAttribute) as CommandParameterAttribute;
                    CommandParametersResultAttribute resultParmAttr = attributes.FirstOrDefault(x => x is CommandParametersResultAttribute) as CommandParametersResultAttribute;

                    if (commandParmAttr != null)
                    {
                        if (prop.PropertyType != typeof(string) && !prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                        {
                            throw new NotSupportedException(string.Format($"Property '{prop.Name}' with type '{prop.PropertyType}' in '{t}' is not supported."));
                        }

                        bool isRequired = Attribute.IsDefined(prop, typeof(RequiredDataAnnotationsAttribute));
                        var value = prop.GetValue(attributedObject);

                        if (isRequired || value != null)
                        {
                            CommandParameterValue parmValue;

                            if (commandNameAttribute.CommandType == CommandType.Request)
                            {
                                parmValue = new CommandParameterValue(ConvertValueToString(commandParmAttr.ValueType, value), DataType.Alphanum);
                            }
                            else
                            {
                                parmValue = new CommandParameterValue(value, commandParmAttr.ValueType);
                            }

                            CommandParameter parm = new CommandParameter(commandParmAttr.Name, parmValue);
                            parameters.Add(parm);
                        }
                    }

                    if (resultParmAttr != null)
                    {
                        results.Add(resultParmAttr.Name, resultParmAttr.ValueType);
                    }
                }

                return new PosnetCommandRequest(commandNameAttribute.CommandType, commandNameAttribute.Name, parameters, results);
            }

            /// <summary>
            /// Converts value to string according to POSNET specification rules.
            /// </summary>
            /// <param name="dataType">A POSNET data type.</param>
            /// <param name="value">A value to convert.</param>
            /// <returns></returns>
            private static string ConvertValueToString(DataType dataType, object value)
            {
                if (value == null)
                {
                    ThrowIf.Null(value, nameof(value));
                }

                var t = value.GetType();
                string stringValue = null;

                if (!_supportedCLRTypes.Contains(t))
                {
                    throw new NotSupportedException(string.Format($"The type {t} is not supported."));
                }

                if (t == typeof(string) && dataType == DataType.Alphanum)
                {
                    stringValue = (string)value;
                }
                else if (t == typeof(DateTime))
                {
                    if (dataType == DataType.Date)
                    {
                        stringValue = ((DateTime)value).ToString("yyyy-MM-dd");
                    }
                    else if (dataType == DataType.DateTime)
                    {
                        stringValue = ((DateTime)value).ToString("yyyy-MM-dd,HH:mm");
                    }
                }
                else if (t == typeof(decimal))
                {
                    if (dataType == DataType.Amount)
                    {
                        stringValue = (Math.Round((decimal)value, 2) * 100).ToString("0.");
                    }
                    else if (dataType == DataType.Num)
                    {
                        decimal decimalValue = (decimal)value;
                        if ((decimalValue - Math.Truncate(decimalValue)) == decimal.Zero)
                        {
                            stringValue = decimalValue.ToString("0.");
                        }
                        else
                        {
                            stringValue = (Math.Round(decimalValue, 3)).ToString("#0.###");
                        }
                    }
                }
                else if (t == typeof(int) && dataType == DataType.Num)
                {
                    stringValue = ((int)value).ToString();
                }
                else if (t == typeof(long) && dataType == DataType.Num)
                {
                    stringValue = ((long)value).ToString();
                }
                else if (t == typeof(bool) && dataType == DataType.Boolean)
                {
                    if ((bool)value)
                    {
                        stringValue = "1";
                    }
                    else
                    {
                        stringValue = "0";
                    }
                }

                if (stringValue == null)
                {
                    throw new NotSupportedException($"The type '{t}' cannot be converted to '{dataType}'.");
                }

                return stringValue;
            }
        }
    }
}
