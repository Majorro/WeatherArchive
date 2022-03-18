using static NPOI.SS.Util.CellReference;

namespace WeatherArchive.Exceptions
{
    /// <summary>
    /// Represents errors that occur during archive file parsing.
    /// </summary>
    internal class InvalidArchiveFileException : Exception
    {
        /// <summary>
        /// The name of the file, where the error occured.
        /// </summary>
        public string? FileName { get; set; }
        /// <summary>
        /// The name of the excel sheet, where the error occured.
        /// </summary>
        public string? SheetName { get; set; }
        /// <summary>
        /// The index of the row (zero-based), where the error occured.
        /// </summary>
        public int? RowNumber { get; set; }
        /// <summary>
        /// The index of the column (zero-based), where the error occured.
        /// </summary>
        public int? ColumnNumber { get; set; }
        /// <summary>
        /// The letter of the excel column, where the error occured.
        /// </summary>
        public string? ColumnLetter => ColumnNumber is null
                                       ? null 
                                       : ConvertNumToColString((int)ColumnNumber);

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArchiveFileException"/> class.
        /// </summary>
        public InvalidArchiveFileException() : 
            base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArchiveFileException"/> class
        /// with a specified error message and data.
        /// </summary>
        /// <param name="message"><inheritdoc cref="Exception.Message" path="/summary"/></param>
        /// <param name="fileName"><inheritdoc cref="FileName" path="/summary"/></param>
        /// <param name="sheetName"><inheritdoc cref="SheetName" path="/summary"/></param>
        /// <param name="rowNumber"><inheritdoc cref="RowNumber" path="/summary"/></param>
        /// <param name="columnNumber"><inheritdoc cref="ColumnNumber" path="/summary"/></param>
        public InvalidArchiveFileException(string? message, string? fileName, string? sheetName, int? rowNumber, int? columnNumber)
            : base(message)
        {
            FileName = fileName;
            SheetName = sheetName;
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArchiveFileException"/> class
        /// with a specified error message, error data and inner exception.
        /// </summary>
        /// <param name="message"><inheritdoc cref="Exception.Message" path="/summary"/></param>
        /// <param name="inner"><inheritdoc cref="Exception.InnerException" path="/summary"/></param>
        /// <param name="fileName"><inheritdoc cref="FileName" path="/summary"/></param>
        /// <param name="sheetName"><inheritdoc cref="SheetName" path="/summary"/></param>
        /// <param name="rowNumber"><inheritdoc cref="RowNumber" path="/summary"/></param>
        /// <param name="columnNumber"><inheritdoc cref="ColumnNumber" path="/summary"/></param>
        public InvalidArchiveFileException(string? message, Exception? inner, string? fileName, string? sheetName, int? rowNumber, int? columnNumber)
            : base(message, inner)
        {
            FileName = fileName;
            SheetName = sheetName;
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArchiveFileException"/> class
        /// with a generated error message based on a specified error data.
        /// </summary>
        /// <param name="fileName"><inheritdoc cref="FileName" path="/summary"/></param>
        /// <param name="sheetName"><inheritdoc cref="SheetName" path="/summary"/></param>
        /// <param name="rowNumber"><inheritdoc cref="RowNumber" path="/summary"/></param>
        /// <param name="columnNumber"><inheritdoc cref="ColumnNumber" path="/summary"/></param>
        public InvalidArchiveFileException(string? fileName, string? sheetName, int? rowNumber, int? columnNumber)
            : base(FormatMessage(fileName, sheetName, rowNumber, columnNumber))
        {
            FileName = fileName;
            SheetName = sheetName;
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArchiveFileException"/> class
        /// with an inner exception and generated error message based on a specified error data.
        /// </summary>
        /// <param name="inner"><inheritdoc cref="Exception.InnerException" path="/summary"/></param>
        /// <param name="fileName"><inheritdoc cref="FileName" path="/summary"/></param>
        /// <param name="sheetName"><inheritdoc cref="SheetName" path="/summary"/></param>
        /// <param name="rowNumber"><inheritdoc cref="RowNumber" path="/summary"/></param>
        /// <param name="columnNumber"><inheritdoc cref="ColumnNumber" path="/summary"/></param>
        public InvalidArchiveFileException(Exception? inner, string? fileName, string? sheetName, int? rowNumber, int? columnNumber)
            : base(FormatMessage(fileName, sheetName, rowNumber, columnNumber), inner)
        {
            FileName = fileName;
            SheetName = sheetName;
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Creates an error message based on a specified data.
        /// </summary>
        /// <param name="fileName"><inheritdoc cref="FileName" path="/summary"/></param>
        /// <param name="sheetName"><inheritdoc cref="SheetName" path="/summary"/></param>
        /// <param name="rowNumber"><inheritdoc cref="RowNumber" path="/summary"/></param>
        /// <param name="columnNumber"><inheritdoc cref="ColumnNumber" path="/summary"/></param>
        /// <returns>An error message.</returns>
        private static string FormatMessage(string? fileName, string? sheetName, int? rowNumber, int? columnNumber) =>
            $"Invalid value in cell {(columnNumber is null ? "<EMPTY>" : ConvertNumToColString((int)columnNumber))}" +
            $"{(columnNumber is null ? "<EMPTY>" : rowNumber + 1)} on the sheet " +
            $"{sheetName ?? "<EMPTY>"} in the file " +
            $"{fileName ?? "<EMPTY>"}.";
    }
}
