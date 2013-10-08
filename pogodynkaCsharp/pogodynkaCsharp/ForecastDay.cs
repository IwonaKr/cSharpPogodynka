using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pogodynkaCsharp
{
    class ForecastDay
    {
        public string period { get; set; }
        public string icon { get; set; }
        public string iconUrl { get; set; }
        public string title { get; set; }
        public string fcttext { get; set; }
        public string fcttextMetric { get; set; }
        public string pop { get; set; }
        public Date data { get; set; }
    }
}
