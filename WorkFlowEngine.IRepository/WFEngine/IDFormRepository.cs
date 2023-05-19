using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;

namespace WorkFlowEngine.IRepository.WFEngine
{
    public interface IDFormRepository
    {
        Task<DFormDomain> GetSurvey(string surveyId);
    }
}
