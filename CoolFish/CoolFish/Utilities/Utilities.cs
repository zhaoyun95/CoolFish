using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using CoolFishNS.Properties;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     A class with utilities that can be used.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        ///     Gets the application path.
        ///     <value>The application path.</value>
        /// </summary>
        public static string ApplicationPath
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        ///     Gets the application version.
        /// </summary>
        public static Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().
                    GetName().Version;
            }
        }

        /// <summary>
        /// Sets up translations for various controls and messages
        /// </summary>
        /// <param name="windowToSet">The Control to search for items to set</param>
        public static void SetLanguage(Control windowToSet)
        {

            string xmlText;
            LocalSettings.Translations.Clear();

            switch (Settings.Default.LanguageIndex)
            {
                case 0:
                    xmlText = Resources.en_US;
                    break;
                case 1:
                    xmlText = Resources.es_ES;
                    break;
                case 2:
                    xmlText = Resources.ru_RU;
                    break;
                case 3:
                    xmlText = Resources.nn_NO;
                    break;
                case 4:
                    xmlText = Resources.fr_FR;
                    break;
                case 5:
                    xmlText = Resources.zh_CHS;
                    break;
                case 6:
                    xmlText = Resources.zh_CHT;
                    break;
                case 7:
                    xmlText = Resources.el_GR;
                    break;
                case 8:
                    xmlText = Resources.de_DE;
                    break;
                case 9:
                    xmlText = Resources.ar;
                    break;
                case 10:
                    xmlText = Resources.nl_NL;
                    break;
                case 11:
                    xmlText = Resources.sv_SE;
                    break;
                default:
                    xmlText = Resources.en_US;
                    break;
            }

            XElement root = XElement.Parse(xmlText);


            foreach (XElement descendant in root.Descendants("Controls"))
            {
                foreach (XElement xElement in descendant.Descendants())
                {
                    object found = windowToSet.FindName(xElement.Name.LocalName);

                    if (found == null)
                    {
                        continue;
                    }


                    var control = found as ContentControl;
                    if (control != null)
                    {
                        var obj = control;
                        obj.Content = xElement.Attribute("Value").Value;
                    }
                    else
                    {
                        var block = found as TextBlock;
                        if (block != null)
                        {
                            var obj = block;
                            obj.Text = xElement.Attribute("Value").Value;
                        }
                        else
                        {
                            var column = found as DataGridTextColumn;
                            if (column != null)
                            {
                                var obj = column;
                                obj.Header = xElement.Attribute("Value").Value;
                            }
                            else
                            {
                                Logging.Log("Unhandled Type Found: " + found.GetType() + " for name: " + xElement.Name.LocalName);
                            }
                        }
                    }
                }
            }
            foreach (
                XElement item in root.Descendants("Dictionary").SelectMany(descendant => descendant.Descendants("Item"))
                )
            {
                if (item.Attribute("English") == null || item.Attribute("Translation") == null)
                {
                    Logging.Write(
                        Resources.MissingTranslation);
                    Settings.Default.LanguageIndex = 0;
                    SetLanguage(windowToSet);
                    return;
                }

                LocalSettings.Translations.Add(item.Attribute("English").Value,
                    item.Attribute("Translation").Value);
            }
        }
    }
}