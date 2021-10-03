using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using VirindiViewService;
using VirindiViewService.Controls;
using VirindiViewService.XMLParsers;

namespace JCDecalToolsPlugin
{
    /// <summary>
    /// This is where all your plugin logic should go.  Public fields are automatically serialized and deserialized
    /// between plugin sessions in this class.  Check out the main Plugin class to see how the serialization works.
    /// </summary>
    public class PluginLogic
    {
        // public fields will be serialized and restored between plugin sessions
        public int Counter = 0;

        // ignore a specific public field
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string PluginAssemblyDirectory;

        // references to our view stuff is kept private so it is not serialized.
        private HudView view;
        private ViewProperties properties;
        private ControlGroup controls;

        private HudButton EchoButton;
        private HudTextBox EchoText;
        private HudStaticText CounterText;
        private HudButton CounterUpButton;
        private HudButton CounterDownButton;

        #region Startup / Shutdown
        /// <summary>
        /// Called once when the plugin is loaded
        /// </summary>
        public void Startup(NetServiceHost host, CoreManager core, string pluginAssemblyDirectory, string accountName, string characterName, string serverName)
        {
            WriteLog($"Plugin.Startup");
            PluginAssemblyDirectory = pluginAssemblyDirectory;
            CreateView();
        }

        /// <summary>
        /// Called when the plugin is shutting down.  Unregister from any events here and do any cleanup.
        /// </summary>
        public void Shutdown()
        {
            EchoButton.Hit -= EchoButton_Hit;
            CounterUpButton.Hit -= CounterUpButton_Hit;
            CounterDownButton.Hit -= CounterDownButton_Hit;

            view.Visible = false;
            view.Dispose();
        }
        #endregion

        #region VVS Views
        /// <summary>
        /// Create our VVS view from an xml template.  We also assign references to the ui elements, as well
        /// as register event handlers.
        /// </summary>
        private void CreateView()
        {
            new Decal3XMLParser().ParseFromResource("JCDecalToolsPlugin.Views.MainView.xml", out properties, out controls);

            // main plugin view
            view = new HudView(properties, controls);

            // ui element references
            // These name indexes in view come from the viewxml from above
            EchoButton = (HudButton)view["EchoButton"];
            EchoText = (HudTextBox)view["EchoText"];
            CounterText = (HudStaticText)view["CounterText"];
            CounterUpButton = (HudButton)view["CounterUpButton"];
            CounterDownButton = (HudButton)view["CounterDownButton"];

            // ui event handlers
            EchoButton.Hit += EchoButton_Hit;
            CounterUpButton.Hit += CounterUpButton_Hit;
            CounterDownButton.Hit += CounterDownButton_Hit;

            // update ui from state
            CounterText.Text = Counter.ToString();
        }

        private void CounterUpButton_Hit(object sender, EventArgs e)
        {
            Counter++;
            CounterText.Text = Counter.ToString();
        }

        private void CounterDownButton_Hit(object sender, EventArgs e)
        {
            Counter--;
            CounterText.Text = Counter.ToString();
        }

        private void EchoButton_Hit(object sender, EventArgs e)
        {
            CoreManager.Current.Actions.AddChatText($"You hit the button! Text was: {EchoText.Text}", 5);
            //CoreManager.Current.Actions.AddChatText(PluginAssemblyDirectory, 5);
        }
        #endregion

        #region Logging
        public void WriteLog(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(PluginAssemblyDirectory, "exceptions.txt"), true))
                {
                    writer.WriteLine($"JCDecalToolsPlugin: {message}");
                    writer.Close();
                }
            }
            catch { }
        }
        #endregion
    }
}
