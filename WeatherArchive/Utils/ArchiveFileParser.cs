using NPOI.SS.UserModel;
using System.Globalization;
using WeatherArchive.Exceptions;
using WeatherArchive.Models;

namespace WeatherArchive.Utils
{
    /// <summary>
    /// Provides methods for parsing weather archive files to collections of <see cref="ArchiveEntry"/>.
    /// </summary>
    public class ArchiveFileParser
    {
        /// <summary>
        /// The first data row in excel files.
        /// </summary>
        public int FirstExcelRow { get; set; } = 4;
        /// <summary>
        /// Format of the date for parsing.
        /// </summary>
        public string DateFormat { get; set; } = "dd.MM.yyyy";

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileParser"/> class
        /// with the predefined configuration.
        /// </summary>
        public ArchiveFileParser() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileParser"/> class and configures it
        /// using the given parameters.
        /// </summary>
        /// <param name="firstExcelRow"><inheritdoc cref="FirstExcelRow" path="/summary"/></param>
        /// <param name="dateFormat"><inheritdoc cref="DateFormat" path="/summary"/></param>
        public ArchiveFileParser(int firstExcelRow, string dateFormat)
        {
            FirstExcelRow = firstExcelRow;
            DateFormat = dateFormat;
        }

        /// <summary>
        /// Parses the given <paramref name="weatherArchiveFiles"/> 
        /// to a <see cref="IEnumerable{T}"/> containing valid instances of <see cref="ArchiveEntry"/> class.
        /// </summary>
        /// <param name="weatherArchiveFiles">The collection of files to be parsed.</param>
        /// <returns>The resulting <see cref="IEnumerable{T}"/> containing valid instances of <see cref="ArchiveEntry"/> class.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="weatherArchiveFiles"/> is null.</exception>
        public IEnumerable<ArchiveEntry> Parse(IEnumerable<IFormFile> weatherArchiveFiles)
        {
            if (weatherArchiveFiles == null)
                throw new ArgumentNullException(nameof(weatherArchiveFiles));

            return weatherArchiveFiles.Select(file => ParseExcelFile(file))
                                      .SelectMany(entries => entries);
        }

        /// <summary>
        /// Parses the given <paramref name="excelFile"/>
        /// to a <see cref="List{T}"/> containing valid instances of <see cref="ArchiveEntry"/> class.
        /// </summary>
        /// <param name="excelFile">The excel file to be parsed.</param>
        /// <returns>The resulting <see cref="List{T}"/> containing valid instances of <see cref="ArchiveEntry"/> class.</returns>
        /// <exception cref="InvalidArchiveFileException">Throws when the given <paramref name="excelFile"/>
        /// contains incorrect data.</exception>
        private List<ArchiveEntry> ParseExcelFile(IFormFile excelFile)
        {
            List<ArchiveEntry> entries = new();
            DataFormatter formatter = new(); // make property?

            using Stream stream = excelFile.OpenReadStream();
            IWorkbook workbook = WorkbookFactory.Create(stream);
            workbook.MissingCellPolicy = MissingCellPolicy.RETURN_BLANK_AS_NULL;

            for (var sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; ++sheetIdx)
            {
                ISheet sheet = workbook.GetSheetAt(sheetIdx);

                for (int rowIdx = FirstExcelRow; rowIdx <= sheet.LastRowNum; ++rowIdx)
                {
                    IRow row = sheet.GetRow(rowIdx);

                    var cellValues = row.Cells.Select(cell => ProcessCell(cell, formatter))
                                              .ToList();

                    if (cellValues.All(cell => string.IsNullOrWhiteSpace(cell))) // skip empty rows
                        continue;

                    string? date = cellValues.ElementAtOrDefault(0);
                    string? time = cellValues.ElementAtOrDefault(1);

                    if (string.IsNullOrWhiteSpace(date) || 
                        !DateTime.TryParseExact(date, DateFormat, null, DateTimeStyles.None, out DateTime dateTime))
                        throw new InvalidArchiveFileException(excelFile.FileName, sheet.SheetName,
                                                              row.RowNum, 0);

                    if (string.IsNullOrWhiteSpace(time) || 
                        !TimeSpan.TryParse(time, out TimeSpan timeSpan))
                        throw new InvalidArchiveFileException(excelFile.FileName, sheet.SheetName,
                                                              row.RowNum, 1);

                    cellValues[0] = (dateTime + timeSpan).ToString();
                    cellValues.RemoveAt(1);

                    if (!ArchiveEntry.TryParse(cellValues.ToArray(), out ArchiveEntry? entry, out int? errorPos))
                        throw new InvalidArchiveFileException(excelFile.FileName, sheet.SheetName,
                                                              row.RowNum, errorPos + 1);

                    entries.Add(entry!);
                }
            }

            return entries;

            static string? ProcessCell(ICell cell, DataFormatter formatter)
            {
                string? cellValue = formatter.FormatCellValue(cell);

                return string.IsNullOrWhiteSpace(cellValue) ? null : cellValue;
            }
        }
    }
}
