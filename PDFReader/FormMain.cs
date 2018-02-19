// 
//     PDF Reader
//
//     3.0     Uses pdftotext.exe to render a plaintext version of a PDF document for a
//             screenreader user.
//     3.0.6    14 June 2013
//              Update settings from previous versions.
//              Prevent updates happening every single time (in case they fail)
//              Global error handling.
//     3.0.7    Fixed update mechanism - was using wrong url.
//      3.1.0   Added colour change option for display area.
//      3.1.1   Fixed PDF handling bug in installer.
//              Added activation and updating DLLs.
//              Fixed colour change bug. 
//      3.2.0  9 October 2015
//              Now supports non-Western languages (e.g. Polish)
//              Updated error handling, so now logs into Event Viewer correctly.
//              Loads PDF asynchronously, so doesn't hang UI.
//              Added TeamViewer download.
//      3.3.0   24 June 2016
//              Fixed minor bug with Colour dialog not being labelled.
//              Open dialog now supports RTF and TXT files. Not sure this is a good idea. Not shipped.
//      4.0.0   Feb 2018
//              Includes OCR using Tesseract. Progress window while opening files. 

// TODO
//      Open DOCX
//      Wrap text from OCR.
//      Open image files from scanners. Register as scanner target.
//      


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFReader
{
    /// <summary>
    /// Main form for program: everything is in here.
    /// </summary>
    public partial class FormMain : Form
    {

        /// <summary>
        /// The WebbIE I18N object. Use this for any I18N you need to do. 
        /// </summary>
        private I18N _i18n;

        /// <summary>
        /// If returned from CheckPdfForPassword then indicates that the
        /// file does not need a password.
        /// </summary>
        private const string NOT_ENCRYPTED = "NOT_ENCRYPTED_iou289798yu";

        /// <summary>
        /// IF returned from CheckPdfForPassword then indicates that the
        /// file needs a password but the user hasn't provided it.
        /// </summary>
        private const string ENCRYPTED_BUT_USER_HAS_NOT_PROVIDED_PASSWORD = "ENCRYPTED_BUT_NO_PASSWORD";

        /// <summary>
        /// String returned by pdfinfo.exe if it tries to read a PDF which requires a
        /// password to open. 
        /// </summary>
        private const string PDF_NEEDS_PASSWORD = "command line error: incorrect password";

        /// <summary>
        /// The thread for importing a PDF file using pdftotext. 
        /// </summary>
        private BackgroundWorker bwOpenFile;

        private Timer tmrCancelChecker;
        
        /// <summary>
        /// Create new main form: use this to open and save PDF/text files.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Prompt user to open a PDF file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = "pdf";
            ofd.FileName = "";
            ofd.Filter = _i18n.GetText("PDF Files") + "|*.pdf;*.rtf;*.txt;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.png;*.gif";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            // We're commited now, can clear old PDF.
            ResetUIForDocLoad();

            Program.InputFile = ofd.FileName;

            StartPDFOpen();
        }

        /// <summary>
        /// Starts trying to open a PDF file, fall back to Tesseract if unsuccessful.
        /// </summary>
        /// <param name="inputFile"></param>
        private void StartPDFOpen()
        {
            // Always check for password input here since we are still in the UI thread. 
            string password = CheckPdfForEncryption(Program.InputFile);
            if (password == ENCRYPTED_BUT_USER_HAS_NOT_PROVIDED_PASSWORD)
            {
                return;
            }
            Program.InputFilePassword = password;

            // Now get on. 
            string extension = System.IO.Path.GetExtension(Program.InputFile).ToUpperInvariant();
            switch (extension)
            {
                // TODO .DOCX, .DOC, why not?
                case ".RTF":
                case ".TXT":
                    // Native format supported by rich text box, might as well load.
                    this.rtbMain.LoadFile(Program.InputFile);
                    break;
                default:
                    // Show progress UI.
                    Program.Progress = new FormProgress();
                    Program.ProgressMessage = "Opening " + System.IO.Path.GetFileName(Program.InputFile);
                    Program.Progress.Show();
                    this.Hide();
                    // Do processing on another thread.
                    bwOpenFile = new BackgroundWorker();
                    bwOpenFile.DoWork += bwOpenFile_DoWork;
                    bwOpenFile.RunWorkerCompleted += bwOpenFile_RunWorkerCompleted;
                    bwOpenFile.RunWorkerAsync();
                    break;
            }
        }

        /// <summary>
        /// Resets everything in preparation for loading a new PDF.
        /// </summary>
        private void ResetUIForDocLoad()
        {
            RichTextBoxLineSpacing.SetLineSpacing(RichTextBoxLineSpacing.SpacingRule.OneAndAHalfSpacing, 0, this.rtbMain);
            rtbMain.SelectionIndent = 10;
            rtbMain.SelectionRightIndent = 10;
            this.Text = _i18n.GetText(this.Name + ".Text");
            rtbMain.Clear();
            Program.Cancel = false;
        }

        /// <summary>
        /// Checks the PDF file indicated by filename for encryption (according to pdfinfo.exe)
        /// and prompts the user for the password if required. Returns the password or NOT_ENCRYPTED
        /// </summary>
        /// <param name="InputFilename"></param>
        public string CheckPdfForEncryption(string InputFilename)
        {
            if (!System.IO.File.Exists(InputFilename))
            {
                throw new System.IO.FileNotFoundException();
            }
            if (System.IO.Path.GetExtension(InputFilename).ToUpperInvariant() != ".PDF")
            {
                // It's not a PDF file, so no password needed. 
                return NOT_ENCRYPTED;
            }
            string nextLine = GetInfoForPdfFile(InputFilename, "");
            // Check to see if PDF needs a password
            if (nextLine.ToLowerInvariant().Contains(PDF_NEEDS_PASSWORD))
            {
                // Needs password. Prompt user until she provides correct password
                // or hits Escape.
                bool passwordCheck = true;
                string userPassword = "";
                while (passwordCheck)
                {
                    userPassword = Microsoft.VisualBasic.Interaction.InputBox(_i18n.GetText("Enter PDF password:"), Application.ProductName, "");
                    if (userPassword.Length == 0)
                    {
                        // User hit cancel. Get out of loop.
                        passwordCheck = false;

                    }
                    else
                    {
                        nextLine = GetInfoForPdfFile(InputFilename, userPassword);
                        if (!nextLine.Contains(PDF_NEEDS_PASSWORD))
                        {
                            // User has correctly entered the user password.
                            passwordCheck = false;
                        }
                    }
                }
                if (userPassword.Length == 0)
                {
                    return ENCRYPTED_BUT_USER_HAS_NOT_PROVIDED_PASSWORD;
                }
                else
                {
                    return userPassword;
                }
            }
            else
            {
                // Nope, need no password.
                return NOT_ENCRYPTED;
            }
        }

        /// <summary>
        /// Attempts to open filename with pdfinfo.exe and returns the result.
        /// Isn't reading the contents of the PDF. 
        /// Instead, shows whether the PDF is protected.
        /// </summary>
        /// <param name="filename">The PDF file to try to open. </param>
        /// <param name="password">Optional password to use to open the PDF. If none, pass ""</param>
        /// <returns>A string containing the StandardError output and the StandardOutput output.</returns>
        private string GetInfoForPdfFile(string filename, string password)
        {
            string outputFile = filename.ToLowerInvariant().Replace(".pdf", ".info.txt");
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.Arguments = "\"" + filename + "\"";
            psi.FileName = Application.StartupPath + "\\pdfinfo.exe";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            string resultError;
            string resultOutput;
            using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                using (System.IO.StreamReader reader = proc.StandardError)
                {
                    resultError = reader.ReadToEnd();
                }
                using (System.IO.StreamReader reader = proc.StandardOutput)
                {
                    resultOutput = reader.ReadToEnd();
                }
            }
            return resultOutput + "\n" + resultError;
        }

        /// <summary>
        /// Save the text in rtbMain to a text file in UTF8.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            var sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CheckPathExists = true;
            sfd.DefaultExt = "txt";
            sfd.FileName = "";
            sfd.Filter = _i18n.GetText("Text Documents") + "|*.txt";
            System.Windows.Forms.DialogResult dr = sfd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                SavePdf(sfd.FileName);
            }
        }

        /// <summary>
        /// Save the text from the PDF as UTF8 text to the filename provided.
        /// </summary>
        /// <param name="filename"></param>
        public void SavePdf(string filename)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
            {
                sw.Write(rtbMain.Text);
                sw.Close();
                sw.Dispose();
            }
        }

        /// <summary>
        /// Returns the plaintext result of opening the PDF, if any.
        /// </summary>
        /// <returns></returns>
        public string PDFText
        {
            get
            {
                return rtbMain.Text;
            }
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Application.ProductName + "\t" + Application.ProductVersion, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Checks for udpates if we haven't already checked today Prevents repeated cycles of update attempts 
        /// if there is a problem with updating. Requires string value "UpdateCheck" created as a property.
        /// Add "CheckForUpdates();" to your Form_Load event.
        /// 18 June 2013
        /// </summary>
        private void CheckForUpdates(string url)
        {
            // Have we checked already today?
            if (Properties.Settings.Default.UpdateCheck != System.DateTime.Now.ToShortDateString())
            {
                // No! Let's do so.
                // First note that we have now checked today.
                Properties.Settings.Default.UpdateCheck = System.DateTime.Now.ToShortDateString();
                Properties.Settings.Default.Save();
                try
                {
                    WebbIE.Updater.CheckForUpdates(url);
                }
                catch (System.Net.WebException)
                {
                    // Not online, server down etc. 
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CheckForUpdates("http://www.webbie.org.uk/pdfreader/updates.xml");
            _i18n = new I18N();
            _i18n.DoForm(this);
            if (Properties.Settings.Default.FontSetByUser)
            {
                System.Drawing.FontStyle fs;
                if (Properties.Settings.Default.FontBold)
                {
                    fs = FontStyle.Bold;
                }
                else
                {
                    fs = FontStyle.Regular;
                }

                rtbMain.Font = new System.Drawing.Font(Properties.Settings.Default.FontName, Properties.Settings.Default.FontSize, fs);
            }

            // Do colouring
            WebbIE.FormColourSelect.SetColourScheme(this, (WebbIE.ColourScheme)Properties.Settings.Default.ColourScheme);

            // Load any PDF file passed to the application.
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                ResetUIForDocLoad();
                Program.InputFile = Environment.GetCommandLineArgs()[1];
                StartPDFOpen();
            }

            tmrCancelChecker = new Timer();
            tmrCancelChecker.Interval = 250;
            tmrCancelChecker.Tick += TmrCancelChecker_Tick;

        }

        private void TmrCancelChecker_Tick(object sender, EventArgs e)
        {
            if (Program.Cancel)
            {
                bwOpenFile.CancelAsync();
                Program.Cancel = false;
            }
        }

        private void mnuHelpManual_Click(object sender, EventArgs e)
        {
            _i18n.ShowHelp();
        }

        /// <summary>
        /// User has requested to change the font. Use standard dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFont_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FontDialog fdMain = new System.Windows.Forms.FontDialog();
            fdMain.ShowApply = false;
            fdMain.AllowSimulations = false;
            fdMain.AllowVerticalFonts = false;
            fdMain.ShowEffects = false;
            fdMain.ShowHelp = false;
            fdMain.Font = (System.Drawing.Font)this.rtbMain.Font.Clone();
            if (fdMain.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                rtbMain.Font = fdMain.Font;
                Properties.Settings.Default.FontSetByUser = true;
                Properties.Settings.Default.FontBold = fdMain.Font.Bold;
                Properties.Settings.Default.FontSize = fdMain.Font.Size;
                Properties.Settings.Default.FontName = fdMain.Font.Name;
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bwOpenFile != null)
            { 
                if (bwOpenFile.IsBusy)
                    bwOpenFile.CancelAsync();
            }
            Properties.Settings.Default.Save();
        }

        private void mnuOptionsColour_Click(object sender, EventArgs e)
        {
            WebbIE.FormColourSelect fcs = new WebbIE.FormColourSelect();
            fcs.SetCurrentSelection(Properties.Settings.Default.ColourScheme);
            fcs.Icon = this.Icon;
            this._i18n.DoForm(fcs);
            if (fcs.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                WebbIE.FormColourSelect.SetColourScheme(this, fcs.colourScheme);
                Properties.Settings.Default.ColourScheme = (int)fcs.colourScheme;
            }
        }

        private void bwOpenFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // We are back on the UI thread! Awesome.
            if (Program.Cancel)
            {
                // Less awesome: user cancelled open process. Ah well. 
            }
            else
            {
                string outputFile = (string)e.Result;
                // If conversion failed, silently fail. TODO Really?
                if (System.IO.File.Exists(outputFile))
                {
                    rtbMain.Text = System.IO.File.ReadAllText(outputFile, Encoding.UTF8);
                    this.Text = _i18n.GetText(this.Name + ".Text") + " - " + System.IO.Path.GetFileNameWithoutExtension(Program.InputFile);
                    System.IO.File.Delete(outputFile);
                }
            }

            // Clean up temp folder.
            DeleteFilesInFolder(GetTempFolderPathAndCreateItIfNecessary());

            // Back to reading mode
            ResetUIAfterDocLoad();
        }

        private void ResetUIAfterDocLoad()
        {
            // Hide progress and show me. 
            Program.Progress.Close();
            this.Visible = true;
            this.rtbMain.Focus();
        }

        /// <summary>
        /// Splits an input PDF file into multiple image files so that Tesseract can OCR them later.
        /// </summary>
        private void SplitInputPDFIntoImages()
        {
            // First, split PDF into images. Use GhostScript.Net
            Ghostscript.NET.GhostscriptVersionInfo _lastInstalledVersion = null;
            Ghostscript.NET.Rasterizer.GhostscriptRasterizer _rasterizer = null;

            int desired_x_dpi = 96;
            int desired_y_dpi = 96;

            string outputPath = System.IO.Path.GetTempPath() + "\\PDFReader";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }
            outputPath += "\\" + Guid.NewGuid().ToString();

            _lastInstalledVersion = Ghostscript.NET.GhostscriptVersionInfo.GetLastInstalledVersion(
                        Ghostscript.NET.GhostscriptLicense.GPL | Ghostscript.NET.GhostscriptLicense.AFPL,
                        Ghostscript.NET.GhostscriptLicense.GPL);

            _rasterizer = new Ghostscript.NET.Rasterizer.GhostscriptRasterizer();

            _rasterizer.Open(Program.InputFile, _lastInstalledVersion, false);

            var pages = new System.Collections.Generic.List<string>();
            for (int pageNumber = 1; pageNumber <= _rasterizer.PageCount; pageNumber++)
            {
                string pageFilePath = System.IO.Path.Combine(outputPath, "Page-" + pageNumber.ToString() + ".png");

                Image img = _rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);
                img.Save(pageFilePath, System.Drawing.Imaging.ImageFormat.Png);

                System.Diagnostics.Debug.Print(pageFilePath);
                pages.Add(pageFilePath);
                //Console.WriteLine(pageFilePath);
            }

        }

        private void DeleteFilesInFolder(string FolderPath)
        {
            string[] filePaths = System.IO.Directory.GetFiles(FolderPath);
            foreach (string filePath in filePaths)
                System.IO.File.Delete(filePath);
        }

        /// <summary>
        /// The path where the program saves temp files. 
        /// </summary>
        /// <returns></returns>
        private string GetTempFolderPathAndCreateItIfNecessary()
        {
            string tempFolder = System.IO.Path.GetTempPath() + "PDFReaderTemp";
            if (!System.IO.Directory.Exists(tempFolder))
            {
                System.IO.Directory.CreateDirectory(tempFolder);
            }
            return tempFolder;
        }

        private void ConvertPDFUsingGhostScriptAndTesseract(string InputFilename, string OutputFilename)
        {
            //gswin32c -q -sDEVICE=tiffgray -dNOPAUSE -sOutputFile=pdfm3-%d.tiff -r300 -dBATCH PDF.pdf
            //gswin32c -q -sDEVICE=tiffgray -dNOPAUSE -sOutputFile=OUTPUTFILES -r300 -dBATCH INPUTFILE

            var pages = new System.Collections.Generic.List<string>();

            // Have we got a PDF?
            if (System.IO.Path.GetExtension(InputFilename.ToUpperInvariant()) == ".PDF")
            {
                // Yes, need to render to a set of images files using GhostScript
                string tempFolder = GetTempFolderPathAndCreateItIfNecessary();
                // Delete existing temp files.
                DeleteFilesInFolder(tempFolder);

                string outputFilePattern = tempFolder + "\\temp0-%d.tiff";

                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.Arguments = "-q -sDEVICE=tiffgray -dNOPAUSE -sOutputFile=\"" + outputFilePattern + "\" -r300 -dBATCH \"" + 
                    Program.InputFile + "\"";
                if (Program.InputFilePassword != "")
                {
                    psi.Arguments += " -sPDFPassword=\"" + Program.InputFilePassword + "\"";
                }
                psi.FileName = Application.StartupPath + "\\gs\\gswin32c.exe";
                psi.CreateNoWindow = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process proc;
                try
                {
                    proc = System.Diagnostics.Process.Start(psi);
                    while (!proc.HasExited)
                    {
                        if (Program.Cancel)
                        {
                            // It's acceptable to use Kill() because we don't care about losing data (there is 
                            // none) and there is no UI for the pdftotext process.
                            proc.Kill();
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch
                {
                    // Probably a problem with pdftotext.exe and security settings on Windows 8?
                    // I guess we could handle it, but what is the user going to do?
                }
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(tempFolder);
                foreach (var f in di.GetFiles("temp0-?.tiff"))
                {
                    pages.Add(f.FullName);
                }
            }
            else
            {
                // Image file, and we assume it is one image file: I am not handling multi-page TIFF
                // files here. 
                // TODO handle multi-page TIFF
                pages.Add(Program.InputFile);
            }

            // HERE IS THE COMMAND LINE FOR GhostScript
            //gswin32c - q - sDEVICE = tiffgray - dNOPAUSE - sOutputFile = pdfm3 -% d.tiff - r300 - dBATCH PDF.pdf

            // Next, OCR each image and write the result to an output file.
            using (Tesseract.TesseractEngine TE = new Tesseract.TesseractEngine(Environment.CurrentDirectory, Program.GetTesseractLanguage()))
            {
                for (int pageNumber = 0; pageNumber < pages.Count; pageNumber++)
                {
                    ExtractTextFromImageUsingTesseractOCR(pages[pageNumber], TE, OutputFilename);
                }
                TE.Dispose();
            } 


        }

        /// <summary>
        /// Returns the text content of the image file provided using the Tesseract engine provided. 
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="Engine"></param>
        /// <returns>Contents of the page in plain text.</returns>
        private void ExtractTextFromImageUsingTesseractOCR(string ImagePath, Tesseract.TesseractEngine Engine, string outputPath)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputPath, true, Encoding.UTF8))
            {
                using (var image = new System.Drawing.Bitmap(ImagePath))
                {
                    using (var pix = Tesseract.PixConverter.ToPix(image))
                    {
                        using (var page = Engine.Process(pix))
                        {
                            //meanConfidenceLabel.InnerText = String.Format("{0:P}", page.GetMeanConfidence());
                            sw.WriteLine(page.GetText());
                        }
                    }
                }
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            return;
        }

        /// <summary>
        /// Converts the input PDF file into a text file using PDFTOTEXT.EXE. Result is the path to the 
        /// output text file. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwOpenFile_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!System.IO.File.Exists(Program.InputFile))
            {
                // No/missing input file!
                e.Cancel = true;
                return;
            }

            string outputFile = System.IO.Path.GetTempFileName() + ".1.txt"; // System.IO.Path.GetFileNameWithoutExtension(Program.InputFile) + ".txt"; 

            // Work out what to do with it. 
            string extension = System.IO.Path.GetExtension(Program.InputFile).ToUpperInvariant();
            switch (extension)
            {
                case ".PDF":
                    // Start with pdftotext.exe
                    OpenPDFWithPDFTOTEXT(Program.InputFile, outputFile, Program.InputFilePassword);
                    // What did we get back? Did we get any text?
                    var fi = new System.IO.FileInfo(outputFile);
                    if (fi.Exists && fi.Length > 10)
                    {
                        // OK, got some output. 
                        System.Diagnostics.Debug.Print("Got some output! " + outputFile);
                    }
                    else if (Program.Cancel)
                    {
                        // User cancelled, get out. 
                        return;
                    }
                    else
                    {
                        // Nope, no text in the output, need to OCR it.
                        outputFile = System.IO.Path.GetTempFileName() + ".2.txt";
                        Program.ProgressMessage = "Scanning (OCR) " + System.IO.Path.GetFileName(Program.InputFile);
                        ConvertPDFUsingGhostScriptAndTesseract(Program.InputFile, outputFile);
                    }
                    break;
                case ".TIFF":
                case ".TIF":
                case ".PNG":
                case ".JPG":
                case ".JPEG":
                case ".BMP":
                case ".GIF":
                    // Need to OCR the image directly. 
                    Program.ProgressMessage = "Scanning (OCR) " + System.IO.Path.GetFileName(Program.InputFile);
                    using (var TE = new Tesseract.TesseractEngine(Environment.CurrentDirectory, Program.GetTesseractLanguage()))
                    {
                        ExtractTextFromImageUsingTesseractOCR(Program.InputFile, TE, outputFile);
                        TE.Dispose();
                    }
                    break;
                default:
                    // Whoops, unsupported file type. Just get out of here.
                    break;
            }
            e.Result = outputFile; 

        }

        /// <summary>
        /// Tries to convert the PDF provided by InputFilename to OutputFilename with
        /// pdftotext.exe. 
        /// </summary>
        /// <param name="InputFilename"></param>
        /// <param name="OutputFilename"></param>
        void OpenPDFWithPDFTOTEXT(string InputFilename, string OutputFilename, string Password)
        {
            Program.ProgressMessage = "Opening PDF " + System.IO.Path.GetFileName(InputFilename);

            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.Arguments = "-eol dos -nopgbrk ";
            psi.Arguments += " -enc UTF-8 ";
            if (Password != NOT_ENCRYPTED)
            {
                psi.Arguments = psi.Arguments + " -upw " + Password + " ";
            }
            psi.Arguments = psi.Arguments + "\"" + InputFilename + "\" \"" + OutputFilename + "\"";

            psi.FileName = Application.StartupPath + "\\pdftotext.exe";
            psi.CreateNoWindow = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process proc;
            try
            {
                proc = System.Diagnostics.Process.Start(psi);
                while (!proc.HasExited)
                {
                    if (Program.Cancel)
                    {
                        // It's acceptable to use Kill() because we don't care about losing data (there is 
                        // none) and there is no UI for the pdftotext process.
                        proc.Kill(); 
                        return;
                    }
                      
                }
                // Could probably check the PDFTOTEXT return codes for something useful...
                // https://linux.die.net/man/1/pdftotext
            }
            catch
            {
                // Probably a problem with pdftotext.exe and security settings on Windows 8?
            }
        }


        #region TeamViewerDownloadMenu
        private string mTeamViewerPath = "";
        private void mnuHelpTeamviewer_Click(object sender, EventArgs e)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            mTeamViewerPath = System.IO.Path.GetTempFileName() + ".exe";
            wc.DownloadFileCompleted += WebClient_DownloadFileCompletedTV;
            wc.DownloadProgressChanged += WebClient_DownloadProgressChangedTV;
            wc.DownloadFileAsync(new System.Uri("http://www.webbie.org.uk/download/TeamViewerQS.exe"), mTeamViewerPath);
        }
        private void WebClient_DownloadFileCompletedTV(object sender, AsyncCompletedEventArgs e)
        {
            System.Diagnostics.Process.Start(mTeamViewerPath);
        }
        void WebClient_DownloadProgressChangedTV(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
        }


        #endregion

        private void mnuFileScan_Click(object sender, EventArgs e)
        {
            WIA.CommonDialog wiaDialog = new WIA.CommonDialog();
            wiaDialog.ShowSelectDevice();
            return;
            WIA.ImageFile wiaImage = wiaDialog.ShowAcquireImage(WIA.WiaDeviceType.ScannerDeviceType);
            string tempPath = System.IO.Path.GetTempFileName() + wiaImage.FileExtension;
            wiaImage.SaveFile(tempPath);
            StartPDFOpen();
        }
    }
}
