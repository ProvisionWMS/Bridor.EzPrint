using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridor.EzPrint.Model
{
    public class Template
    {
        public int Id { get; set; }
        public bool IsProductAcitve { get; set; }
        public bool IsBackdayActive { get; set; }
        public bool IsQuantityActive { get; set; }
        public bool IsNewDescriptionActive { get; set; }
        public bool IsWorkOrderNumberActive { get; set; }
        public bool IsNewProductActive { get; set; }
        public bool IsExpiryDateActive { get; set; }
        public bool IsOriginalProductionLineActive { get; set; }
        public bool IsBarcodeOK { get; set; } //add 2021-05-27
        public string ParentFormatName { get; set; }
        public string ChildFormatName { get; set; }
        public string Description { get; set; }
        public ProductionLine Line { get; set; }
        public IList<Product> Products { get; set; }

        public Template() {
            this.Products = new List<Product>();
        }
    }
}
