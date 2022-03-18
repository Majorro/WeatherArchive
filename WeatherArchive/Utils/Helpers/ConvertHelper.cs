namespace WeatherArchive.Utils.Helpers
{
    /// <summary>
    /// The helper for conversion operations.
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// Works in the same way as <see cref="Convert.ChangeType(object?, Type)"/>,
        /// but can convert non-nullables to the nullables' underlying type without errors.
        /// </summary>
        /// <param name="value"><inheritdoc cref="Convert.ChangeType(object?, Type)" path="/param[@name='value']"/></param>
        /// <param name="conversionType"><inheritdoc cref="Convert.ChangeType(object?, Type)" path="/param[@name='conversionType']"/></param>
        /// <returns><inheritdoc cref="Convert.ChangeType(object?, Type)" path="/returns"/></returns>
        /// <seealso cref="Convert.ChangeType"/>
        public static object? ChangeType(object? value, Type conversionType)
        {
            Type type = conversionType;

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                type = Nullable.GetUnderlyingType(type)!;
            }

            return Convert.ChangeType(value, type);
        }
    }
}
