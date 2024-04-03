using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridor.EzPrint.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductNumber { get; set; }
        public string EnglishDescription { get; set; }
        public string FrenchDescription { get; set; }
        public string BrandName { get; set; }
        public string Description { get { return (Properties.Settings.Default.Language.ToUpper().CompareTo("ANGLAIS") == 0) ? this.EnglishDescription : this.FrenchDescription; } }
        public Template Template { get; set; }
    }
}
