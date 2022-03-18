using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WeatherArchive.Data;
using WeatherArchive.Exceptions;
using WeatherArchive.Models;
using WeatherArchive.Utils;

namespace WeatherArchive.Controllers
{
    /// <summary>
    /// Controller for weather archive views.
    /// </summary>
    public class WeatherArchiveController : Controller // TODO: add throws statements to xmldocs
    {
        private readonly ILogger<WeatherArchiveController> _logger;
        private readonly WeatherArchiveContext _context;
        /// <summary>
        /// The minimum year for entries' filtration.
        /// </summary>
        private const int MinYear = 1950;
        
        /// <summary>
        /// The possible extensions of files that can be uploaded.
        /// </summary>
        public static readonly string[] SupportedFileExtensions = { ".xlsx", ".xls" };
        /// <summary>
        /// The maximum possible total weight of the uploading files.
        /// </summary>
        public static readonly int MaxUploadCapacity = 20 * 1024 * 1024;

        public WeatherArchiveController(ILogger<WeatherArchiveController> logger, WeatherArchiveContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// The home page endpoint.
        /// </summary>
        /// <returns>The <see cref="ViewResult"/> object for the home page rendering.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Renders a view showing uploaded archive entries.
        /// </summary>
        /// <param name="listType"><inheritdoc cref="ArchiveEntry.ListFor" path="/summary"/></param>
        /// <param name="page">The page number of the rendered page.</param>
        /// <param name="pageSize">The number of entries on the rendered page.</param>
        /// <param name="month">If chosen in <paramref name="listType"/>, filters entries by the
        ///                     month component of the <see cref="ArchiveEntry.Timestamp"/> property.</param>
        /// <param name="year">If chosen in <paramref name="listType"/>, filters entries by the
        ///                     year component of the <see cref="ArchiveEntry.Timestamp"/> property.</param>
        /// <returns>A <see cref="Task"/> entity wrapping the view which renders list of archive entries.</returns>
        [HttpGet]
        public async Task<IActionResult> List(ArchiveEntry.ListFor listType, int page = 1, int pageSize = 10,
                                              ArchiveEntry.Month month = ArchiveEntry.Month.January, int year = MinYear)
        {
            ViewData["minYearSelection"] = MinYear;

            TempData["listTypeValue"] = (int)listType; // change to cookies?
            TempData["pageSizeValue"] = pageSize;
            TempData["monthValue"] = (int)month;
            TempData["yearValue"] = year;

            var entries = _context.ArchiveEntries.AsNoTracking();

            switch (listType)
            {
                case ArchiveEntry.ListFor.AllTime:
                    break;
                case ArchiveEntry.ListFor.Months:
                    entries = entries.Where(entry => entry.Timestamp!.Value.Month == (int)month);
                    break;
                case ArchiveEntry.ListFor.Years:
                    entries = entries.Where(entry => entry.Timestamp!.Value.Year == year);
                    break;
            }

            return View(await PaginatedList<ArchiveEntry>.CreateAsync(entries.OrderBy(entry => entry.Timestamp), page, pageSize)) ;
        }

        /// <summary>
        /// The endpoint for archive file uploading.
        /// </summary>
        /// <returns><returns>The <see cref="ViewResult"/> object for the upload page rendering.</returns></returns>
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// Parses <paramref name="archiveFiles"/> to collection of
        /// <see cref="ArchiveEntry"/> which is saved to the database.
        /// </summary>
        /// <param name="archiveFiles">The collection of files to be parsed.</param>
        /// <returns>A <see cref="Task"/> entity wrapping <see cref="RedirectToActionResult"/>
        /// that redirects to the archive list view.</returns>
        [HttpPut]
        public async Task<IActionResult> Upload(IEnumerable<IFormFile> archiveFiles) // TODO: somehow fix the bug when uploading 2 identical files simultaneously(maybe make timestamp PK instead of guid?)
        {
            if(!archiveFiles.Any())
            {
                TempData["errorMessage"] = "Выберите хотя бы один файл.";
                return View();
            }

            if(!archiveFiles.All(file => SupportedFileExtensions.Any(Path.GetExtension(file.FileName).Contains)) ||
                archiveFiles.Sum(file => file.Length) > MaxUploadCapacity)
            {
                TempData["errorMessage"] = $"Загружаемые файлы должны суммарно весить меньше 20MB и иметь одно из расширений: " +
                                           $"{SupportedFileExtensions.Aggregate((lhs, rhs) => lhs+", "+rhs)}.";
                return View();
            }

            ArchiveFileParser parser = new();
            
            try
            {
                var entries = Task.Run(() => parser.Parse(archiveFiles)); // TODO: maybe add parallel batch upload to db

                await _context.BulkMergeAsync(await entries, options => {
                    options.IgnoreOnMergeUpdateExpression = col => new { col.Id };
                    options.AutoMapOutputDirection = false;
                    options.ColumnPrimaryKeyExpression = col => col.Timestamp;
                });
            }
            catch (InvalidArchiveFileException ex)
            {
                TempData["errorMessage"] = ex.FileName is null
                                           ? "Загружен невалидный файл, данные не были изменены."
                                           : $"Загружен невалидный файл {ex.FileName}, данные не были изменены.";

                return View();
            }
            catch (Exception ex) // TODO: add concurrency error handling, see https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/concurrency?view=aspnetcore-6.0#add-a-tracking-property
            {
                TempData["errorMessage"] = "Внутренняя ошибка. Попробуйте ещё раз.";
                return View();
            }

            return RedirectToAction("List", "WeatherArchive");
        }

        /// <summary>
        /// Endpoint for internal exceptions.
        /// </summary>
        /// <returns>A <see cref="ViewResult"/> object that renders the error page.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
