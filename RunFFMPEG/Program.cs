using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    string s = args[1].Trim('"');
                    string fname = Path.GetFileNameWithoutExtension(s);
                    string fpath = Path.GetDirectoryName(s);
                    string outFileName = fpath + "\\" + Path.GetRandomFileName() + ".jpeg";
                    string s3 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=937:y=40\" -y " + outFileName;
                    string s1 = "ffmpeg -i " + s + " -vf drawtext=\"fontfile=/Windows/Fonts/Montserrat-Regular.ttf: text='" + fname + "':fontcolor=white@1.0:box=1:boxcolor=silver@1.0:fontsize=30:x=1000:y=40\" -y " + outFileName;
                    string cmdline = (Path.GetFileName(Path.GetDirectoryName(s)).EndsWith('3') ? s3 : s1);
                    Process proc = new Process();
                    proc.StartInfo.FileName = "CMD.exe";
                    proc.StartInfo.Arguments = "/c " + cmdline;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.WaitForExit();
                    File.Delete(s);
                    File.Move(outFileName, s);
                    return;
                }
                else
                {
                    ShowError("specify file name to convert!");
                }
            }catch(Exception ex)
            {
                ShowError(getExMessage(ex, ""));
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
    }
}
