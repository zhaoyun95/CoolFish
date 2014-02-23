using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using Microsoft.Build.Evaluation;

namespace CoolFishNS.PluginSystem
{
    internal static class PluginManager
    {
        public static Dictionary<string, PluginContainer> Plugins = new Dictionary<string, PluginContainer>();

        internal static bool ShouldPulse = true;

        internal static Thread PluginThread;

        static PluginManager()
        {
            if (!Directory.Exists(Utilities.Utilities.ApplicationPath + "\\Plugins"))
            {
                Directory.CreateDirectory(Utilities.Utilities.ApplicationPath + "\\Plugins");
            }
        }

        internal static void StartPlugins()
        {
            ShouldPulse = true;

            PluginThread = new Thread(PluginPulse) {IsBackground = true};

            PluginThread.Start();
        }

        internal static void StopPlugins()
        {
            ShouldPulse = false; // stop pulsing plugins
            if (!PluginThread.Join(5000)) // wait for the plugin thread to end
            {
                Logging.Log(Resources.FailedToStopPlugins);
            }
            PluginThread = null;
        }

        internal static void ShutDownPlugins()
        {
            foreach (PluginContainer value in Plugins.Values)
            {
                try
                {
                    value.Plugin.OnShutdown(); // call all plugin shutdown procedures
                }
                catch (Exception ex)
                {
                    Logging.Write("Exception shutting down plugin: " + value.Plugin.Name);
                    Logging.Log(ex);
                }
            }
        }

        internal static IEnumerable<IPlugin> GetEnabledPlugins()
        {
            return
                Plugins.Where(plugin => plugin.Value != null && plugin.Value.Enabled).Select(
                    container => container.Value.Plugin);
        }

        internal static void PulseAllPlugins()
        {
            foreach (IPlugin enabledPlugin in GetEnabledPlugins())
            {
                try
                {
                    if (ShouldPulse)
                    {
                        enabledPlugin.OnPulse();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Write(Resources.PluginPulseException, enabledPlugin.Name);
                    Logging.Log(enabledPlugin.Name);
                    Logging.Log(ex);
                    Plugins[enabledPlugin.Name].Enabled = false;
                }
            }
        }


        internal static void PluginPulse()
        {
            while (ShouldPulse)
            {
                PulseAllPlugins();

                Thread.Sleep(1000/60); // About 60 FPS pulses
            }
        }

        internal static void LoadPlugins()
        {
            Plugins.Clear();
            string[] directories = Directory.GetDirectories(Utilities.Utilities.ApplicationPath + "\\Plugins");

            foreach (string directory in directories)
            {
                try
                {
                    string[] projectFiles = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);

                    if (projectFiles.Length > 1)
                    {
                        Logging.Write(
                            "[{0}] Multiple projects detected. Please have only one .csproj file in the main plugin directory.",
                            directory);
                        continue;
                    }

                    string s = projectFiles[0];

                    var globalProperties = new Dictionary<string, string>
                                           {
                                               {"Configuration", "Release"},
                                               {"Platform", "x86"}
                                           };

                    var pc = new ProjectCollection(globalProperties);

                    pc.RegisterLogger(new BasicFileLogger());

                    try
                    {
                        Project loadProject = pc.LoadProject(s, "4.0");
                        bool successfulBuild = loadProject.Build();

                        if (successfulBuild)
                        {
                            ProjectProperty pp = loadProject.GetProperty("OutputPath");
                            ProjectProperty name = loadProject.GetProperty("AssemblyName");
                            ProjectProperty type = loadProject.GetProperty("OutputType");

                            if (name != null && pp != null && type != null)
                            {
                                if (type.EvaluatedValue.Equals("Library"))
                                {
                                    string file = loadProject.DirectoryPath + "\\" + pp.EvaluatedValue +
                                                  name.EvaluatedValue +
                                                  ".dll";
                                    Assembly asm = Assembly.LoadFile(file);

                                    Type[] types = asm.GetTypes();

                                    foreach (Type t in types)
                                    {
                                        if (t.IsClass && typeof (IPlugin).IsAssignableFrom(t))
                                        {
                                            var temp = (IPlugin) Activator.CreateInstance(t);
                                            if (!Plugins.ContainsKey(temp.Name))
                                            {
                                                Plugins.Add(temp.Name, new PluginContainer(temp));
                                                try
                                                {
                                                    temp.OnLoad();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logging.Write("Error loading plugin: {0}", temp.Name);
                                                    Logging.Log(ex);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Logging.Write("[{0}] Output Type is not \"Library\" Please change this setting.",
                                        directory);
                                }
                            }
                            else
                            {
                                Logging.Write(Resources.PluginError, directory);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Write(ex.Message);
                    }


                    pc.Dispose();
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                    Logging.Write(Resources.PluginError, directory);
                }
            }
        }
    }
}