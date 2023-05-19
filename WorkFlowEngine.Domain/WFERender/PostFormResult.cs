using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    public class PostFormResult
    {
        public int Id { get; set; }
        public string postId { get; set; }
        public string formResult { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Status { get; set; }
        public string Filename { get; set; }
        public string MatrixResult { get; set; }
        public string Foldername { get; set; }
        public int FORMID { get; set; }
        public string APPLICANT_NO { get; set; }
        public string COLUMNNAME { get; set; }
    }

}
