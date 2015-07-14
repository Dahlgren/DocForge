using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DocForge.Helpers
{
    class ImageGenerator
    {
        private string imageToPAA;
        private string outputDirectoryPath;
        private string projectPath;

        private Dictionary<string, string> convertedImages = new Dictionary<string, string>();

        public ImageGenerator (string projectPath, string outputDirectoryPath, string imageToPAA)
        {
            this.imageToPAA = imageToPAA;
            this.outputDirectoryPath = outputDirectoryPath;
            this.projectPath = projectPath;
        }

        public string Image(string filePath)
        {
            filePath = this.projectPath + "\\" + filePath;
            try
            {
                if (!File.Exists(filePath)) return null;

                var hash = this.calculateHash(filePath);

                if (this.convertedImages.ContainsKey(hash))
                {
                    return this.convertedImages[hash];
                }

                if (this.convertImage(filePath, this.outputDirectoryPath + "\\public\\images\\" + hash + ".png"))
                {
                    var path = "public/images/" + hash + ".png";
                    this.convertedImages[hash] = path;
                    return path;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }

            return null;
        }

        private string calculateHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        private bool convertImage(string inputFilePath, string outputFilePath)
        {
            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = inputFilePath + " " + outputFilePath;
            // Enter the executable to run, including the complete path
            start.FileName = this.imageToPAA;
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = true;
            start.UseShellExecute = true;
            int exitCode;


            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;

                return exitCode == 0;
            }
        }
}
}
