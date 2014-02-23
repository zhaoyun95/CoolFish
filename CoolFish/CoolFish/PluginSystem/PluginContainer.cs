using System;
using CoolFishNS.Utilities;

namespace CoolFishNS.PluginSystem
{
    internal class PluginContainer
    {
        internal IPlugin Plugin;
        private bool _enabled;

        internal PluginContainer(IPlugin plugin)
        {
            Plugin = plugin;
            _enabled = false;
        }

        internal bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (_enabled)
                    {
                        try
                        {
                            Plugin.OnEnabled();
                        }
                        catch (Exception ex)
                        {
                            Logging.Write("Exception Enabling plugin: " + Plugin.Name);
                            Logging.Log(ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            Plugin.OnDisabled();
                        }
                        catch (Exception ex)
                        {
                            Logging.Write("Exception Enabling plugin: " + Plugin.Name);
                            Logging.Log(ex);
                        }
                    }
                }
            }
        }
    }
}