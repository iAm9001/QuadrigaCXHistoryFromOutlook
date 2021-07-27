using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Configs;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Services
{
    /// <summary>
    /// The <see cref="QuadrigaCxDataParsingService"/> class contains the logic
    /// for reading .TXT email outputs, analyzing and parsing the text file
    /// details, and converting it to various reports and CSV spreadsheets that can
    /// be used by yourself or your accountant for further analysis.
    /// </summary>
    public class QuadrigaCxDataParsingService
    {
        private const string ltc = "ŁTC";
        private const string eth = "ΞTH";
        private const string btc = "XɃT";

        public readonly Regex TradeDataRegex =
            new Regex(@"^(.*?)\/(.*?) Sell Order ID# ([\d]*).* for ([\$\.\d].*?)([A-Za-z].*)$", RegexOptions.Compiled);

        public readonly Regex WithdrawalsRegex =
            new Regex(@"^Your request to withdraw ([\$\d\.].*?)([A-Za-z].*?) using", RegexOptions.Compiled);

        public readonly Regex FundsAddedRegex = new Regex(@"^([\d].*?)([A-Za-z].*?) for", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the class <see cref="QuadrigaCxDataParsingService"/>.
        /// </summary>
        /// <param name="msgConversionConfig">The object that contains basic configuration details about where
        /// to write files to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is passed in for
        /// <paramref name="msgConversionConfig"/>.</exception>
        public QuadrigaCxDataParsingService(MsgConversionConfig msgConversionConfig)
        {
            MsgConversionConfig = msgConversionConfig ?? throw new ArgumentNullException(nameof(msgConversionConfig));
        }

        public MsgConversionConfig MsgConversionConfig { get; }

        /// <summary>
        /// The <see cref="PopulateTradeData"/> method reads the contents of the files passed in,
        /// and returns a collection of <see cref="TradeRecord"/> objects with all information about the trade
        /// populated, with the exception of either the sell or buy amount (due to only one side of the trade being
        /// reflected in the email message). The missing value should be populated or figured out by your accountant
        /// or otherwise by some sort of tax professional.
        /// </summary>
        /// <param name="dataRow">The row that the trading information is located on. Default value is <value>5</value>.
        /// </param>
        /// <param name="paths">The paths to all of the files to be deserialized / converted.</param>
        /// <returns>Returns the collection of trade records from the email records.</returns>
        public IEnumerable<TradeRecord> PopulateTradeData(int dataRow = 5, params string[] paths)
        {
            List<TradeRecord> trades = new List<TradeRecord>();

            foreach (var path in paths)
            {
                var content = File.ReadAllLines(path);
                FileInfo fInfo = new FileInfo(path);

                var matches = this.TradeDataRegex.Match(content[5].Replace(btc, "btc")
                    .Replace(ltc, "ltc")
                    .Replace(eth, "eth"));

                var p1 = matches.Groups[1].Value;
                var p2 = matches.Groups[2].Value;
                var orderNumber = matches.Groups[3].Value;
                var amount = Regex.Replace(matches.Groups[4].Value, @"^[^\d]{1}", string.Empty);
                var currency = matches.Groups[5].Value;
                var creationDate = fInfo.CreationTimeUtc;
                    var trade = new TradeRecord()
                        {
                          dateUtc  = fInfo.CreationTimeUtc.ToString(),
                          comments = content[5],
                          pair1 = p1,
                          pair2 = p2,
                          buyAmount = amount,
                          buyCurrency = currency,
                          sellOrder = orderNumber
                        };

                    if (p1.Equals(currency, StringComparison.InvariantCultureIgnoreCase))
                    {
                        trade.sellCurrency = p2;
                    }
                    else
                    {
                        trade.sellCurrency = p1;
                    }
                    
                    trades.Add(trade);
            }

            return trades;
        }

        /// <summary>
        /// The <see cref="PopulateWithdrawalData"/> method analyzes, parses, and returns a collection
        /// of all withdrawal data extracted from the passed in text files.
        /// </summary>
        /// <param name="dataRow">The row of the text file to search. Default value is <value>4</value></param>
        /// <param name="paths">The paths of all of the withdrawal requests extracted from the passed in
        /// text files.</param>
        /// <returns>Returns a collection of Withdrawal data.</returns>
        public IEnumerable<DepositWithdrawalCombinedRecord> PopulateWithdrawalData(int dataRow = 4, params string[] paths)
        {
            List<DepositWithdrawalCombinedRecord> withdrawals = new List<DepositWithdrawalCombinedRecord>();
            
            foreach (var path in paths)
            {
                var content = File.ReadAllLines(path);
                FileInfo fInfo = new FileInfo(path);

                var matches = this.WithdrawalsRegex.Match(content[dataRow].Replace(btc, "btc")
                    .Replace(ltc, "ltc")
                    .Replace(eth, "eth"));

                var ammount = Regex.Replace(matches.Groups[1].Value, @"^[^\d]{1}", string.Empty);
                var currency = matches.Groups[2].Value;
                var comment = content[dataRow];
                
                var withdrawal = new DepositWithdrawalCombinedRecord()
                {
                    dateUtc  = fInfo.CreationTimeUtc.ToString(),
                    comments = content[dataRow],
                    ammount = ammount,
                    currency = currency,
                    Type = "Withdrawal"
                };
                
                withdrawals.Add(withdrawal);
            }

            return withdrawals;
        }
        
        /// <summary>
        /// The <see cref="PopulateDepositData"/> method analyzes, parses, and returns a collection
        /// of all deposit data extracted from the passed in text files.
        /// </summary>
        /// <param name="dataRow">The row of the text file to search. Default value is <value>4</value></param>
        /// <param name="paths">The paths of all of the deposit requests extracted from the passed in
        /// text files.</param>
        /// <returns>Returns a collection of Deposit data.</returns>
        public IEnumerable<DepositWithdrawalCombinedRecord> PopulateDepositData(int dataRow = 4, params string[] paths)
        {
            List<DepositWithdrawalCombinedRecord> deposits = new List<DepositWithdrawalCombinedRecord>();
            
            foreach (var path in paths)
            {
                var content = File.ReadAllLines(path);
                FileInfo fInfo = new FileInfo(path);

                var matches = this.FundsAddedRegex.Match(content[dataRow].Replace(btc, "btc")
                    .Replace(ltc, "ltc")
                    .Replace(eth, "eth"));

                var ammount = Regex.Replace(matches.Groups[1].Value, @"^[^\d]{1}", string.Empty);
                var currency = matches.Groups[2].Value;
                var comment = content[dataRow];
                
                var deposit = new DepositWithdrawalCombinedRecord()
                {
                    dateUtc  = fInfo.CreationTimeUtc.ToString(),
                    comments = content[dataRow],
                    ammount = ammount,
                    currency = currency,
                    Type = "Deposit"
                };
                
                deposits.Add(deposit);
            }
            return deposits;
        }

        /// <summary>
        /// The <see cref="WriteTradingHistory(string,QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels.TradeRecord[])"/> method will write all of the passed in trading history data
        /// to a local CSV file.
        /// </summary>
        /// <param name="outPath">The file to output trading history to. Default value is
        /// <value>.\Data\FinalReports\trades.csv</value>.</param>
        /// <param name="trades">The trades to write to the local CSV report.</param>
        public void WriteTradingHistory(string outPath = @".\Data\FinalReports\trades.csv", params TradeRecord[] trades)
        {
            using (var writer = new StreamWriter(outPath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = args => true}))
            {
                csv.WriteRecords(trades);
            }
        }
        
        /// <summary>
        /// The <see cref="WriteFundingHistory(string,QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models.ReportModels.DepositWithdrawalCombinedRecord[])"/>
        /// method writes all deposit / withdrawal information to a local CSV file.
        /// </summary>
        /// <param name="outPath">The file to output funding transactions to. Default value is
        /// <value>.\Data\FinalReports\funding.csv</value>.</param>
        /// <param name="fundings">The records to write to the final funding history CSV file.</param>
        public void WriteFundingHistory(string outPath = @".\Data\FinalReports\funding.csv", 
            params DepositWithdrawalCombinedRecord[] fundings)
        {
            using (var writer = new StreamWriter(outPath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = args => true}))
            {
                csv.WriteRecords(fundings);
            }
        }

    }
}