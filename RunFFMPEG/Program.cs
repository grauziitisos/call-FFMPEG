using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RunFFMPEG
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string oN = "", cL = "", iN = "";
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    iN = args[1];
                    string s = args[1].Trim('"');
                    string fname = Path.GetFileNameWithoutExtension(s);
                    string fpath = Path.GetDirectoryName(s);
                    string outFileName = fpath + "\\" + Path.GetRandomFileName() + ".jpeg";
                    oN = outFileName;
                    string s3 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=937:y=40\" -y " + outFileName;
                    string s1 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=1000:y=40\" -y " + outFileName;
                    string cmdline = (Path.GetFileName(Path.GetDirectoryName(s)).EndsWith('3') ? s3 : s1);
                    cL = cmdline;
                    Process proc = new Process();
                    proc.StartInfo.FileName = "CMD.exe";
                    proc.StartInfo.Arguments = "/c " + cmdline;
                    //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    bool hasExited = true;
                        //proc.WaitForExit(50000);
                    int fiftyCounter = 0;
                    while (!proc.HasExited)
                    {
                        Application.DoEvents();
                        Thread.Sleep(100);
                        fiftyCounter += 100;
                        if (fiftyCounter > 50000) { hasExited = false; break; }
                    }
                    if (!hasExited) throw new Exception("ffmpeg did not exit after 50 secs of waiting!!" +
                         Environment.NewLine + "input: " + args[1] +
                         Environment.NewLine + "tempfile: " + outFileName +
                         Environment.NewLine + "ffmpeg command: " + cmdline +
                         Environment.NewLine);
                    if (WaitForFile(s, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 20, 1000))
                    {
                        File.Delete(s);
                        File.Move(outFileName, s);
                    }
                    else
                    {
                        throw new Exception("unable to write to the input file after 20 secs of waiting!!" +
                         Environment.NewLine + "input: " + args[1] +
                         Environment.NewLine + "tempfile: " + outFileName +
                         Environment.NewLine + "ffmpeg command: " + cmdline +
                         Environment.NewLine);
                    }
                    return;
                }
                else
                {
                    ShowError("specify file name to convert!");
                }
            }catch(Exception ex)
            {
                ShowError(getExMessage(new Exception((Environment.NewLine + "input: " + iN +
                         Environment.NewLine + "tempfile: " + oN +
                         Environment.NewLine + "ffmpeg command: " + cL +
                         Environment.NewLine), ex), ""));
            }
        }

        private static string getExMessage(Exception ex, string prevMsg)
        {
            return ex.InnerException != null ? prevMsg + Environment.NewLine + getExMessage(ex.InnerException, prevMsg) : prevMsg + Environment.NewLine + ex.Message; 

        }
        private static void ShowError(string ex)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TheForm(ex));
        }

        private static bool WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share, int maxTries, int waitTime)
        {
            for (int numTries = 0; numTries < maxTries; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    fs.Dispose();
                    return true;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(waitTime);
                }
            }

            return false;
        }
    }
}
