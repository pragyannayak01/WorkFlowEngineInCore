using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFEngine
{
    public class Districts
    {
        /// <summary>
        /// District Id
        /// </summary>
        public int DIVISION_CODE { get; set; }
        /// <summary>
        /// District Name
        /// </summary>
        public string DIVISION_NAME { get; set; }
        /// <summary>
        /// State Id
        /// </summary>
        public int? StateId { get; set; }
        /// <summary>
        /// Deleted flag : 1-deleted, 0-active
        /// </summary>
        /// 
        public int intlevelid { get; set; }
        public bool bitStatus { get; set; }
    }
}
