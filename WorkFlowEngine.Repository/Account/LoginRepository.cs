using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.IRepository.Account;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.Repository.DapperConfiguration;

namespace WorkFlowEngine.Repository.Account
{
    public class LoginRepository : RepositoryBase, ILoginRepository
    {
        public LoginRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public async Task<IEnumerable<WorkFlowEngine.Domain.Account.Login>> GetLoginDetails(WorkFlowEngine.Domain.Account.Login logins)
        {
            try
            {
                //string password = CommonFunction.EncryptData("gad@123");
                string pass = CommonFunction.DecryptData("wVHwqbgWUadTC5C388Fahg==");
                var dyParam = new DynamicParameters();
                dyParam.Add("P_USERNAME", logins.VCHUSERNAME);
                dyParam.Add("P_PASSWORD", CommonFunction.EncryptData(logins.vchpassword));
                dyParam.Add("P_ACTION", "Login");
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<WorkFlowEngine.Domain.Account.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
