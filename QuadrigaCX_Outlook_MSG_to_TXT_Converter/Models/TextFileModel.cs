using System;
using System.IO;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models
{
    /// <summary>
    /// The <see cref="TextFileModel"/> class represents a unit describing
    /// where to export a .TXT file to, what date to list it's creation date,
    /// and the contents of the file itself. Encoding is UTF8.
    /// </summary>
    public class TextFileModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileModel"/> class. This type contains
        /// all of the required parameters to export your MSG file to a .TXT file suitable to be scanned
        /// for trading information.
        /// </summary>
        /// <param name="creationTime">The date to assign to the CreationTime of the new .TXT file.</param>
        /// <param name="outputDirectoryPath">The folder that will be used to write all of the .TXT files to.</param>
        /// <param name="messageContents">The body of the text file to output.</param>
        /// <param name="fileName">The name of the .TXT file to create. It will be created in the directory
        /// provided for the <paramref name="outputDirectoryPath"/> parameter.</param>
        /// <exception cref="ArgumentException">Thrown when any of the constructor parameters
        /// are null, or otherwise whitespace.</exception>
        public TextFileModel(DateTime? creationTime, string outputDirectoryPath, string messageContents, string fileName)
        {
            if (string.IsNullOrWhiteSpace(outputDirectoryPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputDirectoryPath));
            if (string.IsNullOrWhiteSpace(messageContents))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(messageContents));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            if (!creationTime.HasValue)
            {
                throw new ArgumentException("A valid DateTime object must be passed in, date cannot be null.");
            }
            this.CreationTime = creationTime.Value;
            this.OutputDirectoryPath = outputDirectoryPath;
            this.MessageContents = messageContents;
            this.FileName = fileName;

            this.CreateRequiredFolders(this.OutputDirectoryPath);
        }

        private void CreateRequiredFolders(string path)
        {
            string fundsAdded = Path.Combine(path, "FundsAdded");
            string withdrawals = Path.Combine(path, "Withdrawal");
            string filledOrders = Path.Combine(path, "FilledOrders");
            string partialOrders = Path.Combine(path, "PartialOrders");
            string finalReports = Path.Combine(path, "FinalReports");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(fundsAdded)) Directory.CreateDirectory(fundsAdded);
            if (!Directory.Exists(withdrawals)) Directory.CreateDirectory(withdrawals);
            if (!Directory.Exists(filledOrders)) Directory.CreateDirectory(filledOrders);
            if (!Directory.Exists(partialOrders)) Directory.CreateDirectory(partialOrders);
            if (!Directory.Exists(finalReports)) Directory.CreateDirectory(finalReports);

            /*
            mkdir FundsAdded
mkdir Withdrawal
mkdir FilledOrders
mkdir PartialOrders
             


             */
        }

        /// <summary>
        /// Gets the <see cref="CreationTime"/> to assign to the .TXT file being created.
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// Gets the <see cref="OutputDirectoryPath"/> of the file that will be created.
        /// </summary>
        public string OutputDirectoryPath { get; }

        /// <summary>
        /// Gets the contents of the message to be analyzed.
        /// </summary>
        public string MessageContents { get; }

        /// <summary>
        /// Gets the name of the file to create. It will be created in the folder
        /// referenced in the property <see cref="OutputDirectoryPath"/>.
        /// </summary>
        public string FileName { get; }
    }
}