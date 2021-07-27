using System;
using System.IO;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter.Configs
{
    /// <summary>
    /// The <see cref="MsgConversionConfig"/> class represents the parameters required
    /// in order for the application to know where to read your QuadrigaCX .MSG file contents from,
    /// where to write the .TXT converted files to, etc.
    /// </summary>
    public class MsgConversionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgConversionConfig"/> class.
        /// </summary>
        /// <param name="msgFilesPath"></param>
        /// <param name="txtOutFilePath"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileFormatException"></exception>
        public MsgConversionConfig(string msgFilesPath, string txtOutFilePath)
        {
            if (string.IsNullOrWhiteSpace(msgFilesPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(msgFilesPath));
            if (string.IsNullOrWhiteSpace(txtOutFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(txtOutFilePath));

            if (!Directory.Exists(msgFilesPath))
            {
                throw new DirectoryNotFoundException("Please enter the full or relative path " +
                                                     "to a directory containing the .MSG files that represent" +
                                                     "your QuadrigaCX trading data. The directory " + 
                                                     $"{msgFilesPath} could not be found.");
            }
            else if (!File.GetAttributes(msgFilesPath).HasFlag(FileAttributes.Directory))
            {
                throw new FileFormatException("You have provided the path directly to a single file. Please " +
                                              "provide the path to the folder where the .MSG files containing your " +
                                              "QuadrigaCX .MSG files are located.");
            }

            if (Directory.Exists(txtOutFilePath) &&
                !File.GetAttributes(txtOutFilePath).HasFlag(FileAttributes.Directory))
            {
                throw new FileFormatException("You have provided the path directly to a single .TXT file. " +
                                              "Please provide the path to the folder only where the .TXT files  + " +
                                              "containing your QuadrigaCX trading information are to be written to. " +
                                              "If teh directory does not exist, it will be created for you.");
            }
            else if (!Directory.Exists(txtOutFilePath))
            {
                Directory.CreateDirectory(txtOutFilePath);
            }

            this.MsgFilesPath = msgFilesPath;
            this.TxtOutFilePath = txtOutFilePath;
        }

        /// <summary>
        /// Gets the directory where all new .MSG files containing QuadrigaCX trading information
        /// are located.
        /// </summary>
        public string MsgFilesPath { get;  }
        
        /// <summary>
        /// Gets the directory where all new .TXT files converted from .MSG QuadrigaCX trading related emails
        /// will be written to.
        /// </summary>
        public string TxtOutFilePath { get; }
    }
}