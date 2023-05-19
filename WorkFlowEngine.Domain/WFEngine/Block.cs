using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFEngine
{
    public class Block
    {
        /// <summary>
        /// BLOCK Id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// BLOCK CODE
        /// </summary>
        public int DISTRICT_CODE { get; set; }
        /// <summary>
        /// BLOCK Name
        /// </summary>
        public string DISTRICT_NAME { get; set; }
        /// <summary>
        /// District Id
        /// </summary>
        public int DIST_ID { get; set; }
    }

}
