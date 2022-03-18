using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WeatherArchive.Utils
{
    /// <summary>
    /// Provides a convenient interface for paginated data of type <typeparamref name="T"/>
    /// from arbitrary data source.
    /// </summary>
    /// <typeparam name="T">The type of paginated elements.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Index of the page for which a <see cref="PaginatedList{T}"/> instance stores elements.
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// Number of elements on a single page.
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// The list of possible values for the <see cref="PaginatedList{T}.PageSize"/>.
        /// </summary>
        public SelectList PageSizeList { get; private set; }
        /// <summary>
        /// Number of all pages.
        /// </summary>
        public int TotalPages { get; private set; }
        /// <summary>
        /// Indicates whether the current page is first or not.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;
        /// <summary>
        /// Indicates whether the current page is last or not.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class that
        /// contains elements copied from the specified <see cref="List{T}"/> instance which
        /// size must be equal to <paramref name="pageSize"/>.
        /// Also assignes properties by values, based on passed parameters.
        /// </summary>
        /// <param name="items">The elements for the current page.</param>
        /// <param name="count">The number of all existing elements in a data source.</param>
        /// <param name="pageIndex"><inheritdoc cref="PageIndex" path="/summary"/></param>
        /// <param name="pageSize"><inheritdoc cref="PageSize" path="/summary"/></param>
        /// <param name="pageSizeList"><inheritdoc cref="PageSizeList" path="/summary"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Throws when the given <paramref name="pageSize"/> is 
        /// not in <paramref name="pageSizeList"/>.</exception>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, SelectList pageSizeList) // TODO: add more checks
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            PageSizeList = pageSizeList;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (!pageSizeList.Any(item => item.Value == pageSize.ToString()))
                throw new ArgumentOutOfRangeException(nameof(pageSize),
                    pageSize, "The given page size is not in the page size list.");

            AddRange(items);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PaginatedList{T}"/> class for the
        /// <paramref name="dataSource"/>.
        /// </summary>
        /// <param name="dataSource">The data source of elements to be stored in the <see cref="PaginatedList{T}"/>.</param>
        /// <param name="pageIndex"><inheritdoc cref="PageIndex" path="/summary"/></param>
        /// <param name="pageSize"><inheritdoc cref="PageSize" path="/summary"/></param>
        /// <param name="pageSizeList"><inheritdoc cref="PageSizeList" path="/summary"/></param>
        /// <returns>A new <see cref="PaginatedList{T}"/> instance wrapped into <see cref="Task"/> instance.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> dataSource, int pageIndex, int pageSize, SelectList? pageSizeList = null)
        {
            var count = await dataSource.CountAsync();
            var items = await dataSource.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize,
                pageSizeList ?? new SelectList(new object[]
                {
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "50", Value = "50" },
                    new SelectListItem { Text = "100", Value = "100" }
                }, "Value", "Text", pageSize.ToString()));
        }
    }
}