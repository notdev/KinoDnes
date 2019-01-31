using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KinoDnesApi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataGenerator _dataGenerator;
        public IEnumerable<string> Cities;

        public IndexModel(DataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public void OnGet()
        {
            Cities = _dataGenerator.GetCities();
        }
    }
}