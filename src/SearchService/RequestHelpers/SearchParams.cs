using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchService.RequestHelpers
{
    public class SearchParams
    {
        public string searchTerm { get; set; }
        public int pageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public string Seller { get; set; }
        public string Winner { get; set; }
        public string OrderBy { get; set; }
        public string FilterBy { get; set; }
    }
}