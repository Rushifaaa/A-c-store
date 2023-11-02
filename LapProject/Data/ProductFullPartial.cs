using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LapProject.Extensions;

namespace LapProject.Data
{
    public partial class ProductFull
    {
        public decimal GrossUnitPrice { get { return NetUnitPrice * (100m + TaxRate)/100m; } }
        public string GrossUnitPriceString { get { return GrossUnitPrice.ToEuroString(); } }
    }
}