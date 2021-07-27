namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels
{
    
    public class TradeRecord {
        public string pair1 { get; set;}
        public string pair2{ get; set;}
        public string sellOrder{ get; set;}
        public string buyAmount{ get; set;}
        public string buyCurrency{ get; set;}
        public string sellAmmount{ get; set;}
        public string sellCurrency{ get; set;}
        public string dateUtc{ get; set;}
        public string comments{ get; set;}
        public string fee { get; set;}
        public string feeCurrency { get; set;}
    }
}