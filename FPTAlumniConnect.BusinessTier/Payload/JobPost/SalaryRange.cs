using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.JobPost
{
    public class SalaryRange
    {
        public int? Min { get; set; }
        public int? Max { get; set; }

        public bool OverlapsWith(int? jobMin, int? jobMax)
        {
            if (!Min.HasValue && !Max.HasValue) return true;
            if (!Min.HasValue) return jobMin <= Max;
            if (!Max.HasValue) return jobMax >= Min;
            return jobMin <= Max && jobMax >= Min;
        }
    }


}
