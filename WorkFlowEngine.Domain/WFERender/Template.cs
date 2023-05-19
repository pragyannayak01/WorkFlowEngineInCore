using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    //[Table("JK_TBL_TEMPLATE")]
    public class Template //: BaseEntity<int>
    {
        public int TEMPLATEID { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public int COLUMNID { get; set; }
        public string COLUMNNAME { get; set; }
        public string CSS_FILE { get; set; }
        //public override int Key
        //{
        //    get { return TEMPLATEID; }
        //    set { TEMPLATEID = value; }
        //}
    }
}
