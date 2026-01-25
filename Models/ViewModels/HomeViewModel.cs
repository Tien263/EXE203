using System.Collections.Generic;
using Exe_Demo.Models;

namespace Exe_Demo.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; } = new List<Product>();
        public IEnumerable<Product> NewProducts { get; set; } = new List<Product>();
    }
}
