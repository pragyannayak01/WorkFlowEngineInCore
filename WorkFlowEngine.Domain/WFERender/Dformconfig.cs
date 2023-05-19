using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkFlowEngine.Domain.WFERender
{
    public class Dformconfig
    {
        public int int_id { get; set; }
        public int int_form_id { get; set; }
        public int int_panel_id { get; set; }
        public string pvch_control_name { get; set; }
        public string pvch_control_type { get; set; }
        public string pvch_control_id { get; set; }
        public string pvch_label_name { get; set; }
        public int pint_length { get; set; }
        public string pvch_validationtype { get; set; }
        public int pint_reqvalidation { get; set; }
        public string pvch_tooltip { get; set; }
        public int pint_automapping { get; set; }
        public string pvch_textmode { get; set; }
        public string pvch_cssclass { get; set; }
        public string pvch_datasource { get; set; }
        public string pvch_datavaluefield { get; set; }
        public string pvch_datatextfield { get; set; }
        public string pvch_fileallowed { get; set; }
        public int pint_maxsize { get; set; }
        public string pvch_option { get; set; }
        public string pvch_defaultvalue { get; set; }
        public string pvch_headingtext { get; set; }
        public string pvch_pluginid { get; set; }
        public string pvch_child_control_name { get; set; }
        public string pvch_child_label_name { get; set; }
        public int FormId { get; set; }
        public int ConfigControlId { get; set; }
        public int Col_Cnt { get; set; }
        public XElement ColumnXml { get; set; }
    }

}
