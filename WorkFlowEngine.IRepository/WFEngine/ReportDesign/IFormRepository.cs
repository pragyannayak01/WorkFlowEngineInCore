using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFERender;

namespace WorkFlowEngine.IRepository.WFEngine.ReportDesign
{
    public interface IFormRepository
    {
        Task<IEnumerable<Forms>> GetAllForms(int DeptId);

        Task<IEnumerable<Forms>> GetallFormColumnById(int DeptId);
    }
}
