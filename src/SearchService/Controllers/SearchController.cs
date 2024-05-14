using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm, int pageNumber=1, int pageSize=4)
        {
            var query = DB.PagedSearch<Item>();

            query.Sort(b => b.Ascending(s => s.Make));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }

            query.PageNumber(pageNumber);
            query.PageSize(pageSize);

            var result = await query.ExecuteAsync();
            if (result.TotalCount == 0)
                return NotFound("No result matches the search term.");
            return Ok(new{
                result = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}