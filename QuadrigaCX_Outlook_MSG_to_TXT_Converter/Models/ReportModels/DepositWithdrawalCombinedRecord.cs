namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels
{
    public class DepositWithdrawalCombinedRecord
    {

        public string Type { get; set; }
        public string ammount { get; set; }
        public string currency { get; set; }
        public string dateUtc { get; set; }
        public string comments { get; set; }
        public string fee { get; set; }
        public string feeCurrency { get; set; }
    }
}