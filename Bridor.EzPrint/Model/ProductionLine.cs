using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridor.EzPrint.Model
{
    public class ProductionLine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrinterName { get; set; }
        public Plant Plant { get; set; }
        public IList<Template> Templates { get; set; }

        public ProductionLine() {
            this.Templates = new List<Template>();
        }
    }
}
