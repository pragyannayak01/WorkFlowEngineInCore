using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFEngine
{
    public class Forms
    {
        public int ID { get; set; }
        public int DFID { get; set; }
        public string FORMID { get; set; }
        public string SCHEME_NAME { get; set; }
        public string INSERT_NAME { get; set; }
        public string UPDATE_NAME { get; set; }
        public string DELETE_NAME { get; set; }
        public string SELECT_NAME { get; set; }
        public int DEPTID { get; set; }

        public string DEPTNAME { get; set; }
        public string TYPE { get; set; }

        public string GOAL_IMAGE { get; set; }
        public string SCRIPT_FILE { get; set; }

        public int TEMPLATEID { get; set; }
        public string TABLE_NAME { get; set; }
        public string FORMTYPE { get; set; }
        public virtual DFormDomain Form { get; set; }

        public int INT_ID { get; set; }

        public string PVCH_LABEL_NAME { get; set; }
        public bool IsChecked { get; set; }
        public string JSONSTRING { get; set; }
        public string PRINTCONFIG { get; set; }
    }

}
