using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MsgKit;
using MsgKit.Enums;
using MsgReader.Outlook;
using NUnit.Framework;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Configs;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Models;
using QuadrigaCX_Outlook_MSG_to_TXT_Converter.Services;
using AddressType = MsgKit.Enums.AddressType;
using MessageImportance = MsgReader.Outlook.MessageImportance;

namespace QuadrigaCX_Outlook_MSG_to_TXT_Converter_Tests
{
    [TestFixture]
    public class MsgConversionServiceTests
    {
        public MsgConversionServiceTests()
        {
            System.Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            try
            {
                Directory.CreateDirectory(@".\Data\TestData");
            }
            catch {}
        }

        [TestCase(@".\Data", @".\Data", ExpectedResult = true)]
        public bool GetMsgFilePathsTest(string msgPath, string txtPath)
        {
            var config = new MsgConversionConfig(msgPath, txtPath);
            var service = new MsgConversionService(config);

            var msgFiles1 = service.GetMsgFilePaths(config.MsgFilesPath);
            var msgFiles2 = service.GetMsgFilePaths();

            CollectionAssert.AreEquivalent(msgFiles1, msgFiles2);

            return (msgFiles1.Length > 1 && msgFiles2.Length > 2);
        }

        [TestCase(@".\Data", @".\Data")]
        public void ReadMsgFileTest(string msgFilesPath, string txtFilesPath)
        {
            var config = new MsgConversionConfig(msgFilesPath, txtFilesPath);
            var service = new MsgConversionService(config);

            var paths = service.GetMsgFilePaths();

            foreach (var path in paths)
            {
                var txtFileModel = service.ReadMsgFile(path);
            }
        }

        [TestCase(@".\Data", @".\Data")]
        public void WriteTxtFileTest(string msgFilesPath, string txtFilesPath)
        {
            var config = new MsgConversionConfig(msgFilesPath, txtFilesPath);
            var service = new MsgConversionService(config);

            var paths = service.GetMsgFilePaths();

            foreach (var path in paths)
            {
                var txtFileModel = service.ReadMsgFile(path);
                service.WriteTxtFile(txtFileModel);
            }
        }

        [TestCase(@"C:\temp\quadrigaTest\Data", @".\Data\TestData", "youremail@yourdomain.com")]
        public void GenerateTestMsgFiles(string msgFilesPath, string testDataPath, string newRecipient)
        {
            var replaceStrings = TestContext.Parameters["StringsToReplace"]
                .Split(new [] { ";"}, StringSplitOptions.RemoveEmptyEntries);

            var config = new MsgConversionConfig(msgFilesPath, msgFilesPath);
            var service = new MsgConversionService(config);
            var parseService = new QuadrigaCxDataParsingService(config);

            var paths = service.GetMsgFilePaths();
            List<Storage.Message> inputMsgs = new List<Storage.Message>();
            List<Message> outputMsgs = new List<Message>();

            foreach (var path in paths)
            {
                FileInfo currentFile = new FileInfo(path);
                using (var msg = new MsgReader.Outlook.Storage.Message(path))
                {
                    using (var email = new Email(
                        new Sender(msg.Sender.Email, msg.Sender.DisplayName, AddressType.Smtp),
                        "WillBeReplaced"))
                    {
                        email.Recipients.AddTo("you@yourdomain.com");
                        email.Subject = msg.Subject;
                        email.SentOn = msg.SentOn;
                        email.BodyHtml = msg.BodyHtml;
                        email.BodyText = msg.BodyText;

                        foreach (var s in replaceStrings)
                        {
                            string randomStringVal = TestContext.CurrentContext.Random.Next(10000,20000).ToString();
                            email.BodyText = email.BodyText.Replace(s, randomStringVal);
                            email.BodyHtml = email.BodyHtml.Replace(s, randomStringVal);
                        }

                        string randomStringVaTxt = TestContext.CurrentContext.Random.Next(10000).ToString();

                        email.BodyText = Regex.Replace(email.BodyText, @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}",
                            randomStringVaTxt);
                        email.BodyHtml = Regex.Replace(email.BodyHtml, @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}",
                            randomStringVaTxt);
                        

                        //email.Importance = MsgKit.Enums.MessageImportance.IMPORTANCE_NORMAL;
                        //email.IconIndex = MessageIconIndex.;
                     //   email.MessageEditorFormat = MessageEditorFormat.EDITOR_FORMAT_HTML;
                    //    email.Priority = MessagePriority.PRIO_NORMAL;
                    
                        email.Save(Path.Combine(testDataPath, $"{currentFile.Name}"));
                    }
                }

            }
        }
    }
}