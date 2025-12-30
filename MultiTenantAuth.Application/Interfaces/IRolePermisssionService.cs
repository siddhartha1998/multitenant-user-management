using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Interfaces
{
    public interface IRolePermisssionService
    {
        Task<IReadOnlyCollection<string>> GetPermissionsAsync(List<string> roles);
    }
}
