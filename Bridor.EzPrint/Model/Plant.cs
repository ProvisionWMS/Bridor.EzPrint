using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridor.EzPrint.Model
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<ProductionLine> ProductionLines { get; set; }

        public Plant() {
            ProductionLines = new List<ProductionLine>();
        }
    }
}
