using NPOI.SS.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using WeatherArchive.Exceptions;
using WeatherArchive.Utils.Helpers;

namespace WeatherArchive.Models
{
    /// <summary>
    /// The model describing a weather archive entry.
    /// </summary>
    public class ArchiveEntry : BaseModel
    {
        [Required]
        [Display(Name = "Отметка времени")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}")]
        public DateTime? Timestamp { get; set; }

        [Required]
        [Display(Name = "Температура воздуха, гр. Ц.")]
        public double? Temperature { get; set; }

        [Required]
        [Display(Name = "Отн. влажность воздуха, %")]
        public double? RelativeHumidity { get; set; }

        [Required]
        [Display(Name = "Точка росы, гр. Ц.")]
        public double? DewPoint { get; set; }

        [Required]
        [Display(Name = "Атм. давление, мм рт.ст.")]
        public int? AtmospherePressure { get; set; }

        [Display(Name = "Направление ветра")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? WindDirection { get; set; }

        [Display(Name = "Скорость ветра, м/с")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? WindSpeed { get; set; }

        [Display(Name = "Облачность, %")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? Cloudiness { get; set; }

        [Display(Name = "Нижняя граница облачности, м")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? CloudBase { get; set; }

        [Display(Name = "Горизонтальная видимость, км")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? HorizontalVisibility { get; set; }

        [Display(Name = "Погодные явления")]
        [DisplayFormat(NullDisplayText = "-")]
        public string? WeatherСonditions { get; set; }

        /// <summary>
        /// <para>
        ///     Creates a new object of type <see cref="ArchiveEntry"/> and 
        ///     assignes converted values from <paramref name="propertiesValues"/> to its properties
        ///     using either the default reflection index-to-property mapping or provided <paramref name="propertyMapping"/>.
        /// </para>
        /// 
        /// <para>
        ///     Also returns a value that indicates whether the conversion succeeded.
        /// </para>
        /// </summary>
        /// <param name="propertiesValues">The values, to be converted and assigned to a new <see cref="ArchiveEntry"/> object.</param>
        /// <param name="resultEntry">When the method returns, in case of success contains valid <see cref="ArchiveEntry"/> object,
        /// otherwise <c>null</c>.</param>
        /// <param name="errorPos">When the method returns, in case of success contains <c>null</c>,
        /// otherwise the index of an invalid value in accordance with the used mapping.</param>
        /// <param name="propertyMapping">Index-to-property mapping to be used in <see cref="ArchiveEntry"/> properties assignment.
        /// If not provided, then used default reflection mapping.</param>
        /// <returns><c>true</c>, if success, otherwise <c>false</c>.</returns>
        public static bool TryParse(string?[] propertiesValues, out ArchiveEntry? resultEntry, out int? errorPos, PropertyInfo[]? propertyMapping = null)
        {
            errorPos = null;
            resultEntry = new ArchiveEntry();

            propertyMapping ??= typeof(ArchiveEntry).GetProperties(BindingFlags.Public |
                                                                   BindingFlags.DeclaredOnly |
                                                                   BindingFlags.Instance);

            for(var propertyIdx = 0; propertyIdx < propertyMapping.Length; ++propertyIdx)
            {
                PropertyInfo property = propertyMapping[propertyIdx];
                string? value = propertyIdx < propertiesValues.Length ? propertiesValues[propertyIdx] : null;
                bool isRequired = property.CustomAttributes
                                          .Any(c => c.AttributeType == typeof(RequiredAttribute));

                if (isRequired && string.IsNullOrWhiteSpace(value))
                {
                    errorPos = propertyIdx;
                    resultEntry = null;
                    return false;
                }

                try
                {
                    property.SetValue(resultEntry,
                        ConvertHelper.ChangeType(value, property.PropertyType));
                }
                catch
                {
                    errorPos = propertyIdx;
                    resultEntry = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Defines the way of archive filtration.
        /// </summary>
        public enum ListFor
        {
            [Display(Name = "За все время")]
            AllTime,
            [Display(Name = "По месяцам")]
            Months,
            [Display(Name = "По годам")]
            Years
        }

        /// <summary>
        /// Months to be used in archive filtration.
        /// </summary>
        public enum Month
        {
            [Display(Name = "Январь")]
            January = 1,
            [Display(Name = "Февраль")]
            February,
            [Display(Name = "Март")]
            March,
            [Display(Name = "Апрель")]
            April,
            [Display(Name = "Май")]
            May,
            [Display(Name = "Июнь")]
            June,
            [Display(Name = "Июль")]
            July,
            [Display(Name = "Август")]
            August,
            [Display(Name = "Сентябрь")]
            September,
            [Display(Name = "Октябрь")]
            October,
            [Display(Name = "Ноябрь")]
            November,
            [Display(Name = "Декабрь")]
            December
        }
    }
}
