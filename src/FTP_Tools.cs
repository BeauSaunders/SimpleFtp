using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SimpleFtp
{
    public class FtpClient
    {
        static string uri = "ftp://127.0.0.1";
        static string user = "anonymous";
        static string pass = string.Empty;

#region CORE

        /// <summary>
        /// Sets the passed paramters to memory (internal variables are static so only need to be set once)
        /// </summary>
        /// <param name="_uri">IP/Path. E.g: "ftp://127.0.0.1"</param>
        /// <param name="_user">FTP Username</param>
        /// <param name="_pass">FTP Password</param>
        /// <param name="_boolConnection">[Default:true] Performs a connection test after setting</param>
        public static async Task SetCredentials(string _uri, string _user, string _pass, bool _boolConnection = true)
        {
            uri = _uri;
            user = _user;
            pass = _pass;

            Console.WriteLine("Credentials set.");

            if (_boolConnection)
                await TestConnectionAsync();
        }

        /// <summary>
        /// Sends an FTP request using the specified method and returns the raw FtpWebResponse.
        /// </summary>
        /// <param name="method">The FTP method to execute. Must be a valid type, such as WebRequestMethods.Ftp.ListDirectory.</param>
        /// <returns>The FtpWebResponse returned by the server.</returns>
        public static async Task<FtpWebResponse> GetResponseAsync(string method, string path = "", int timeout = 5000, int readWriteTimeout = 5000)
        {
            // still valid for ftp so suppress the warning
            #pragma warning disable SYSLIB0014
            FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(uri + "/" + path);
            #pragma warning restore SYSLIB0014

            ftpReq.Credentials = new NetworkCredential(user, pass);
            ftpReq.Method = method;
            ftpReq.Timeout = timeout;
            ftpReq.ReadWriteTimeout = readWriteTimeout;

            return (FtpWebResponse)await ftpReq.GetResponseAsync();
        }

        /// <summary>
        /// Attempts a simple 'List Directory' call to test the connection.
        /// </summary>
        /// <param name="log">[Default:true] If true -> The method will log status updates to the Console</param>
        /// <returns>Bool: Whether the Connection Test was successful or not.</returns>
        public static async Task<bool> TestConnectionAsync(bool log = true)
        {
            try
            {
                // make a simple request to the root and get the response
                using FtpWebResponse res = await GetResponseAsync(WebRequestMethods.Ftp.ListDirectory);

                // Any successful status code means the connection works
                bool success = res.StatusCode == FtpStatusCode.OpeningData ||
                            res.StatusCode == FtpStatusCode.DataAlreadyOpen ||
                            res.StatusCode == FtpStatusCode.CommandOK;
                
                if (log)
                    Console.WriteLine("Connection test was successful.");
                
                return success;
            }
            catch
            {
                // Any exception means connection failed
                if (log)
                    Console.WriteLine("Connection test failed!");

                return false;
            }
        }

#endregion
#region  FUNCTIONS

        /// <summary>
        /// Attempts to get a list of strings of all directories at the given path.
        /// </summary>
        /// <param name="path">[Default:""] Path from root to query</param>
        /// <returns>List of strings of directory names.</returns>
        public static async Task<List<string>> GetDirectoriesAsync(string path = "")
        {
            List<string> dirs = new();

            using FtpWebResponse ftpResponse = await GetResponseAsync(WebRequestMethods.Ftp.ListDirectory, path);
            using StreamReader sReader = new StreamReader(ftpResponse.GetResponseStream());

            string? l = await sReader.ReadLineAsync(); // read 1st line
            while (!string.IsNullOrEmpty(l)) // if there is a string at this line
            {
                dirs.Add(l); // add this string (name of folder) to the list
                l = await sReader.ReadLineAsync(); // read the next line
            }

            return dirs;
        }

        /// <summary>
        /// Downloads the data from a file on the FTP server at the given path and returns the file's contents
        /// </summary>
        /// <param name="path">Path from root to query</param>
        /// <returns>string (the file contents)</returns>
        public static async Task<string> GetFileContentsAsync(string path)
        {
            using FtpWebResponse ftpResponse = await GetResponseAsync(WebRequestMethods.Ftp.DownloadFile, path);
            using StreamReader sReader = new StreamReader(ftpResponse.GetResponseStream());

            return await sReader.ReadToEndAsync();
        }

        /// <summary>
        /// Downloads the file on the ftp server at the given path and writes it to the local path
        /// </summary>
        /// <param name="localPath">The file to write to on the local machine (e.g. dir/subdir/file.txt)</param>
        /// <param name="path">Path from root to query</param>
        public static async Task DownloadFileAsync(string localPath, string path)
        {
            await File.WriteAllTextAsync(localPath, await GetFileContentsAsync(path));
        }

#endregion

    }
}