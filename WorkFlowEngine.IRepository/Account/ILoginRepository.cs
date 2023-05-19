using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.IRepository.Account
{
    public interface ILoginRepository
    {
        Task<IEnumerable<WorkFlowEngine.Domain.Account.Login>> GetLoginDetails(WorkFlowEngine.Domain.Account.Login logins);
    }
}
