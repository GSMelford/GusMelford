using Bot.Api.Collection;

namespace Bot.Api.BotRequests
{
    using System;
    using System.Reflection;
    using Interfaces;
    
    public abstract class SerializeParameters : IParameters
    {
        public virtual ParameterCollection BuildParameters()
        {
            ParameterCollection parameters = new ParameterCollection();
            Type parametersObject = GetType();
            
            foreach (PropertyInfo property in parametersObject.GetProperties())
            {
                object[] attrs = property.GetCustomAttributes(false);
                object value = property.GetValue(this);

                if (attrs[0] is not ParameterNameAttribute attribute)
                    continue;
                
                if (!CheckDefaultParameterValue(value))
                {
                    parameters.Add(attribute.ParameterName, value, attribute.UrlEncode);
                }
                else if(attribute.IsRequired)
                {
                    throw new ArgumentException($"{nameof(SerializeParameters)} error: " +
                                                $"Property {property.Name} has to set");
                }
            }
            
            return parameters;
        }

        private bool CheckDefaultParameterValue(object value)
        {
            switch (value)
            {
                case null:
                    return true;
                case ValueType:
                {
                    object obj = Activator.CreateInstance(value.GetType());
                    return obj?.Equals(value) ?? false;
                }
                default:
                    return false;
            }
        }
    }
}