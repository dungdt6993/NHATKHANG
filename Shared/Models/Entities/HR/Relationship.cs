using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Relationship
    {
        public int RelationshipID { get; set; }
        public string RelationshipName { get; set; }
    }
}
