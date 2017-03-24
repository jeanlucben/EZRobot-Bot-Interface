///    Synbot Plugin Plugin for EZB Builder Software intefacing With Synthetic Intelligence Network SYNBOT Framework
///    Copyright(C) 2016  Jean-Luc BENARD
///    This program is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.
///    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
///    GNU General Public License for more details.
///    You should have received a copy of the GNU General Public License along with this program.If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EZ_Builder.Config.Sub;
using System.IO;

namespace SynbotPlugin
{
    public partial class ConfigurationForm : Form
    {

        public ConfigurationForm()
        {
            InitializeComponent();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog1.Description =
            "Select the directory that you want to use ";
            // Do not allow the user to create new files via the FolderBrowserDialog.
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // Reserved for future Use so invisible
            EZ_Builder.Invokers.SetVisible(groupBox3, false);
        }
        public void SetConfiguration(PluginV1 cf)
        {     
            textBox4.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_USER_ID].ToString();
            textBox1.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_PATH].ToString();
            textBox2.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_SAVE_PATH].ToString();
            textBox5.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_DEFAULT_ANSWER_MESSAGE].ToString();
            comboBox1.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_DEFAULT_LANGUAGE].ToString();
            textBox8.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_ROBOTTYPE].ToString();
            textBox7.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_RESPONSE_VARIABLE].ToString();
            ucScriptResponseSynbot.Value = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_VALUE].ToString();
            ucScriptResponseSynbot.XML = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_XML].ToString();
            checkBox3.Checked = Convert.ToBoolean(cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_RESPONSE]);
            checkBox1.Checked = Convert.ToBoolean(cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_EZB]);
            checkBox2.Checked = Convert.ToBoolean(cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_EXEC_BOTCOMMAND]);
            textBox10.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_SYNBOTSERVER_IPADRESS].ToString();
            textBox11.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_SYNBOTSERVER_TCPPORT].ToString();
            ucScriptBotStart.Value = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_VALUE].ToString();
            ucScriptBotStart.XML = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_XML].ToString();
            ucScriptBotStop.Value = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_VALUE].ToString();
            ucScriptBotStop.XML = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_XML].ToString();
            textBox17.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_WELCOME_MESSAGE].ToString();
            textBox16.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_INIT_EVENT].ToString();
            textBox6.Text = cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_BEFORE_STOP_EVENT].ToString();
            checkBox4.Checked = Convert.ToBoolean(cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_FULLLOG]);
        }
       
        public PluginV1 GetConfiguration()
        {
            PluginV1 cf = new PluginV1();
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_USER_ID, textBox4.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_PATH, textBox1.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_SAVE_PATH, textBox2.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_DEFAULT_ANSWER_MESSAGE, textBox5.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_DEFAULT_LANGUAGE, comboBox1.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_ROBOTTYPE, textBox8.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_RESPONSE_VARIABLE, textBox7.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_VALUE, ucScriptResponseSynbot.Value);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_XML, ucScriptResponseSynbot.XML);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_RESPONSE, checkBox3.Checked);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_EZB, checkBox1.Checked);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_FLAG_EXEC_BOTCOMMAND, checkBox2.Checked);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_SYNBOTSERVER_IPADRESS, textBox10.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_SYNBOTSERVER_TCPPORT, textBox11.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_VALUE, ucScriptBotStart.Value);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_XML, ucScriptBotStart.XML);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_VALUE, ucScriptBotStop.Value);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_XML, ucScriptBotStop.XML);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_WELCOME_MESSAGE, textBox17.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_INIT_EVENT, textBox16.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_BEFORE_STOP_EVENT, textBox6.Text);
            cf.STORAGE.AddOrUpdate(ConfigurationDictionary._STORAGE_BOT_FLAG_FULLLOG, checkBox4.Checked);
            return cf;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                EZ_Builder.Invokers.SetText(textBox1, folderBrowserDialog1.SelectedPath);
            }
        }
        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                EZ_Builder.Invokers.SetText(textBox2, folderBrowserDialog1.SelectedPath);
            }
        }
    }
}
