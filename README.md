# QuadrigaCXHistoryFromOutlook
This C# application will allow you to extract your QuadrigaCX trading history from your Outlook inbox, parsing data from all emails you've ever received from QuadrigaCX.

It should be noted that this will not be a 100% completely accurate representation of your trading history. It has the following deficiencies:

  * If you cannot infer your account balance with a specific range of dates that your trades occurred between, you will need to use your own logic (realistically, advice from your accountant) on how to best calculate the value of these trades. Unfortunately the information available from the Filled Orders + Partial Orders email notifications do **not** tell you how much of an assett you bought or sold, only the amount that you bought it for.
    * As such, the script (at this time) makes no attempt to calculate the quantity of units bought / sold of the asset that you traded. 
  * If you were unlucky and had money stolen from you by QuadrigaCX, you need to be aware of exactly how much of each asset you lost in the theft. This script has no way of knowing or understanding how much money /crypto you had stolen from you based on the email trails.

## Steps
1. Unzip the release to a local folder
2. Create a subfolder called Data
3. Select all of your email from QuadrigaCX in Outlook, and drag them from Outlook into the Data subfolder (this will save every single email as a .MSG file)
4. Run QuadrigaCX_Outlook_MSG_to_TXT_Converter.exe from the folder that you extracted it to
5. When you look at the Data folder, you should see new subfolders created, and each subfolder should contain .TXT representations of your emails, of particular interest is the "FinalReports" folder.
6. Validate all of the results in the FinalReport folder. 
7. The results should be perfect, with the exception of the report **trades.csv**. Due to the fact that the QuadrigaCX Filled Orders / Partial Filled order emails do not contain the amount of the assett that you sold for the second trading pair, this makes it difficult to calculate accurately; you will have to do this yourself. That being said, with all of the data you now have at your disposal, you are probably MUCH farther along in your journey than you were before you began! There is probably enough information to paint a complete picture for your accountant at the very least, who could then advise you on decisions to make for declaring trade prices.

### Requirements: .NET Framework 4.8
