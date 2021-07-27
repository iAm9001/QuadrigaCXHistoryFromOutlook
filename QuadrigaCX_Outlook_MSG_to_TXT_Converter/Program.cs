using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Configs;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Services;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            var config = new MsgConversionConfig(@".\Data", @".\Data");
            var service = new MsgConversionService(config);
            var outService = new QuadrigaCxDataParsingService(config);

            var paths = service.GetMsgFilePaths();

            foreach (var path in paths)
            {
                var txtFileModel = service.ReadMsgFile(path);
                service.WriteTxtFile(txtFileModel);
            }

            string withdrawalsPath = Path.Combine(config.TxtOutFilePath, "Withdrawal");
            string depositsPath = Path.Combine(config.TxtOutFilePath, "FundsAdded");
            string filledPath = Path.Combine(config.TxtOutFilePath, "FilledOrders");
            string partialsPath = Path.Combine(config.TxtOutFilePath, "PartialOrders");
            
            var withdrawals = Directory.GetFiles(withdrawalsPath, "*.txt");
            var deposits = Directory.GetFiles(depositsPath, "*.txt");
            var filledTrades = Directory.GetFiles(filledPath, "*.txt");
            var partialTrades = Directory.GetFiles(partialsPath,  "*.txt");

            var _withdrawals = outService.PopulateWithdrawalData(4, withdrawals);
            var _deposits = outService.PopulateDepositData(4, deposits);
            var _filledTrades = outService.PopulateTradeData(5, filledTrades);
            var _partialTrades = outService.PopulateTradeData(5, partialTrades);

            var fundings = new List<DepositWithdrawalCombinedRecord>();
            fundings.AddRange(_withdrawals);
            fundings.AddRange(_deposits);

            var trades = new List<TradeRecord>();
            trades.AddRange(_filledTrades);
            trades.AddRange(_partialTrades);
            
            outService.WriteFundingHistory(fundings: fundings.ToArray());
            outService.WriteTradingHistory(trades: trades.ToArray());
        }
    }
}
