using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
                    TimeSpan idleTime = GetIdleTime();
                    //throw new Exception("Current idle time is: " + idleTime.ToString(@"dd\.hh\:mm\:ss\.ff"));
                   string s = args[1].Trim('"');
                    iN = args[1];
                    string fname = Path.GetFileNameWithoutExtension(s);
                    string fpath = Path.GetDirectoryName(s);
                    string outFileName = fpath + "\\" + Path.GetRandomFileName() + ".jpeg";
                    oN = outFileName;
                    string s3 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=937:y=40\" -y " + outFileName;
                    string s1 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=1000:y=40\" -y " + outFileName;
                    string cmdline = (Path.GetFileName(Path.GetDirectoryName(s)).EndsWith('3') ? s3 : s1);
                    cL = cmdline;
                    if (idleTime.TotalDays > 1 ? false : idleTime.TotalHours >1 ? false: idleTime.Minutes < 5) {
                        //Put the timestamp on the non-idle screenshot
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
                        if(!File.Exists(s)) throw new Exception("File does not exist!!!" +
                             Environment.NewLine + "input: " + args[1] +
                             Environment.NewLine + "tempfile: " + outFileName +
                             Environment.NewLine);
                        //delete the idle screens
                        if (WaitForFile(s, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 20, 1000))
                        {
                            //wait for the closing and disposing to happen...
                            Thread.Sleep(1000);
                            File.Delete(s);
                        }
                        else
                        {
                            throw new Exception("unable to delete the input file after 20 secs of waiting!!" +
                             Environment.NewLine + "input: " + args[1] +
                             Environment.NewLine + "tempfile: " + outFileName +
                             Environment.NewLine);
                        }
                    }
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

        #region imports
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetTickCount();

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
        #endregion

        #region utils
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
                    using (fs = new FileStream(fullPath, mode, access, share))
                    {
                        //fs.Close();
                        //fs.Dispose();
                        return true;
                    }
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
        private static TimeSpan GetIdleTime()
        {
            TimeSpan idleTime = TimeSpan.FromMilliseconds(0);

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                idleTime = TimeSpan.FromMilliseconds(GetTickCount() - (lastInputInfo.dwTime & uint.MaxValue));
                //idleTime = TimeSpan.FromSeconds(Convert.ToInt32(lastInputInfo.dwTime / 1000));
            }

            return idleTime;
        }
        #endregion
    }
}
