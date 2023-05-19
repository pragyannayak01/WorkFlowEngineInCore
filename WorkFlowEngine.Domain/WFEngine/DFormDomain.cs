
namespace WorkFlowEngine.Domain.WFEngine
{
    public class DFormDomain
    {
        public int ID { get; set; }
        public int SchemeId { get; set; }
        public string FORMID { get; set; }
        public string JSONSTRING { get; set; }
        public Forms scheme { get; set; }
    }
}
