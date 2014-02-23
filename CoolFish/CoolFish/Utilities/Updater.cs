using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Windows.Forms;
using CoolFishNS.Properties;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     All Web Request related functions are in this class (Updating, ShouldRun, News).
    /// </summary>
    internal static class Updater
    {
        private static bool _ignoreDevCheck;

        /// <summary>
        ///     Gets a value indicating whether the bot [should run].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [should run]; otherwise, <c>false</c>.
        /// </value>
        public static bool ShouldRun
        {
            get
            {
                if (_ignoreDevCheck)
                {
                    return true;
                }

                try
                {
                    WebRequest request = WebRequest.Create("http://coolfish.unknowndev.com/ShouldRun.php?version=" + Utilities.Version);

                    request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
                    WebResponse response = request.GetResponse();

                    Stream stream = response.GetResponseStream();

                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);

                        string result = reader.ReadToEnd();

                        return result.Contains("true");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Write(LocalSettings.Translations["ShouldRun Error"]);

                    Logging.Log(ex);


                    DialogResult ret =
                        MessageBox.Show(
                            Resources.ShouldRunException,
                            "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (ret == DialogResult.Yes)
                    {
                        _ignoreDevCheck = true;
                        return true;
                    }
                }
                return false;
            }
        }


        /// <summary>
        ///     Gets the news from the svn message.
        /// </summary>
        /// <returns></returns>
        public static string GetNews()
        {
            try
            {
                WebRequest request = WebRequest.Create("http://unknowndev.github.io/CoolFish/Message.txt");

                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);

                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logging.Write("Could not connect to news feed. Website is down?");
                Logging.Log(ex);
            }
            return string.Empty;
        }

        public static void Update()
        {
           
            try
            {
                WebRequest request = WebRequest.Create("http://unknowndev.github.io/CoolFish/LatestVersion.txt");
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);

                    string info = reader.ReadToEnd();

                    Version ver = new Version(info);

                    if (ver > Utilities.Version)
                    {
                        Logging.Write(LocalSettings.Translations["Current Revision"] + ": " + Utilities.Version +
                                      " " + LocalSettings.Translations["Latest Revision"] + ": " + ver);

                        DialogResult result =
                            MessageBox.Show(LocalSettings.Translations["New Version Available"],
                                LocalSettings.Translations["New Version"], MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            DownloadNewVersion();
                        }
                        return;
                    }
                }
                Logging.Write(LocalSettings.Translations["No New Revisions Available"]);
            }
            catch (Exception ex)
            {
                Logging.Log(ex.Message);
                Logging.Log(ex);
            }
        }

        private static void DownloadNewVersion()
        {
            try
            {
                Process.Start("https://github.com/unknowndev/CoolFish/releases");
            }
            catch (Exception ex)
            {
                Logging.Write("https://github.com/unknowndev/CoolFish/releases");
                Logging.Log(ex);
            }
        }
    }
}