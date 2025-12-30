using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantAuth.Domain.Enums
{
    public enum UserScope
    {
        System = 1,
        Tenant = 2,
        Merchant = 3,
        Outlet = 4
    }
}
