using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KapeRest.Core.Entities.Branch
{
    public class BranchEntities
    {
        public int Id { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
      
    }
}
