using System;
using System.IO;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Configs;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Services
{
    /// <summary>
    /// The <see cref="MsgConversionService"/> class contains the logic for reading QuadrigaCX email
    /// MSG files from Microsoft Outlook, as well as for converting those files to TXT in the output location.
    /// </summary>
    public class MsgConversionService
    {
        private const string ltc = @"ŁTC";
        private const string eth = @"ΞTH";
        private const string btc = @"XɃT";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgConversionService"/> class, which is responsible
        /// for providing .MSG to .TXT conversion capabilities for your QuadrigaCX trading informatin.
        /// </summary>
        /// <param name="config">The configuration opbject with implementation details required for the
        /// service class to perform it's operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null configuration is passed into
        /// the classes' constructor.</exception>
        public MsgConversionService(MsgConversionConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets the <see cref="MsgConversionConfig"/> object that contains information required to perform
        /// read and write operations for .MSG to .TXT file operations.
        /// </summary>
        public MsgConversionConfig Config { get; }
        
        /// <summary>
        /// The <see cref="ReadMsgFile"/> reads the contents of the .MSG file provided in the path, and returns an
        /// object containing details on how to create the converted .TXT files representing yoru QuadrigaCX
        /// trading history emails.
        /// </summary>
        /// <param name="path">The full or relatibe path to a single .MSG file.</param>
        /// <returns>Returns the <see cref="TextFileModel"/> object containing details about
        /// how and where to create the .TXT file representing your QuadrigaCX trading data.</returns>
        /// <exception cref="ArgumentException">Thrown when a null or whitespace value is passed in
        /// instead of a valid path to a .MSG file.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the path to the .MSG file provided does
        /// not exist.</exception>
        public TextFileModel ReadMsgFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            FileInfo fInfo = new FileInfo(path);

            string messageContent = string.Empty;
            
            using (var msg = new MsgReader.Outlook.Storage.Message(path))
            {
                var sentOn = msg.SentOn;
                messageContent = msg.BodyText;

                string newFName = Path.Combine(fInfo.Name.Replace(fInfo.Extension, ".txt"));

                var fileModel = new TextFileModel(sentOn, this.Config.TxtOutFilePath, messageContent,
                    newFName);

                return fileModel;
            }
        }
        
        /// <summary>
        /// The <see cref="WriteTxtFile"/> method writes the contents of the provided <see cref="TextFileModel"/>
        /// argument to the file indicated within the <see cref="TextFileModel.OutputDirectoryPath"/> property.
        /// </summary>
        /// <param name="model">The model containing information about the trading data to write to the
        /// .TXT file for later analysis.</param>
        public void WriteTxtFile(TextFileModel model)
        {
            string newPath = model.OutputDirectoryPath;
            if (model.FileName.StartsWith("Funds Added"))
            {
                newPath = Path.Combine(newPath, "FundsAdded");
            }
            else if (model.FileName.StartsWith("Order Filled"))
            {
                newPath = Path.Combine(newPath, "FilledOrders");
            }
            else if (model.FileName.StartsWith("Order Partially Filled"))
            {
                newPath = Path.Combine(newPath, "PartialOrders");
            }
            else if (model.FileName.StartsWith("Withdrawal Request Received"))
            {
                newPath = Path.Combine(newPath, "Withdrawal");
            }
            string pathToNewFile = Path.Combine(newPath, model.FileName);
            FileInfo fInfo = new FileInfo(pathToNewFile);
            using (var fw = fInfo.CreateText())
            {
                fw.Write(model.MessageContents);
            }
            fInfo.CreationTime = model.CreationTime;
        }

        /// <summary>
        /// The <see cref="GetMsgFilePaths(string)"/> method returns all of the full path names
        /// of .MSG files contained within the service classes' <see cref="Config"/> property.
        /// </summary>
        /// <returns>Returns the full path to all located .MSG files.</returns>
        public string[] GetMsgFilePaths()
        {
            return this.GetMsgFilePaths(this.Config.MsgFilesPath);
        }
        
        /// <summary>
        /// The <see cref="GetMsgFilePaths(string)"/> method returns all of the full path names
        /// of .MSG files contained within the passed in <paramref name="msgDirectoryPath"/> directory.
        /// </summary>
        /// <param name="msgDirectoryPath">The path name of the folder containing the .MSG files whose full
        /// path and filenames will be returned.</param>
        /// <returns>Returns the full path to all located .MSG files.</returns>
        public string[] GetMsgFilePaths(string msgDirectoryPath)
        {
            return Directory.GetFiles(msgDirectoryPath, "*.msg", SearchOption.TopDirectoryOnly);
        }
    }
}