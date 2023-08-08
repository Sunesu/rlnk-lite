using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rlnk
{

    class IconResponse{
        public List<Icon> data { get; set; }
    }

    public class Icon{
        public Int64 targetId {get; set;}
        public string state { get; set; }
        public string imageUrl { get; set; }
    }
}
