using System;
using D69soft.Shared.Models.Entities.SYSTEM;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class PermissionUserVM : PermissionUser
    {
        public int PermisId { get; set; }

        public string Description { get; set; }
    }
}
