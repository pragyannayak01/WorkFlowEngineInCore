using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.IRepository.WFEngine;
using WorkFlowEngine.Repository.DapperConfiguration;

namespace WorkFlowEngine.Repository.WFEngine
{
    public class DFormRepository : RepositoryBase, IDFormRepository
    {
        public DFormRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public async Task<DFormDomain> GetSurvey(string surveyId)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            try
            {
                var query = "select * from DF_FORM where ID=" + surveyId;
                var dyParam = new DynamicParameters();
                var result = await Connection.QueryAsync<DFormDomain>(query, dyParam, commandType: CommandType.Text);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
