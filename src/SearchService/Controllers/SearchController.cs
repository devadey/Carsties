using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            ;

            if (!string.IsNullOrEmpty(searchParams.searchTerm))
            {
                query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(b => b.Ascending(s => s.Make)),
                "new" => query.Sort(b => b.Descending(s => s.CreatedAt)),
                _ => query.Sort(b => b.Ascending(s => s.AuctionEnd))
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(b => b.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(b => b.AuctionEnd < DateTime.UtcNow.AddHours(6) && b.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(b => b.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }

            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }

            query.PageNumber(searchParams.pageNumber);
            query.PageSize(searchParams.PageSize);

            var result = await query.ExecuteAsync();
            if (result.TotalCount == 0)
                return NotFound("No result matches the search term.");
            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}