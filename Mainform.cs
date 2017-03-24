///    Synbot Plugin Plugin for EZB Builder Software intefacing With Synthetic Intelligence Network SYNBOT Framework
///    Copyright(C) 2016  Jean-Luc BENARD
///    This program is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.
///    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
///    GNU General Public License for more details.
///    You should have received a copy of the GNU General Public License along with this program.If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using EZ_Builder;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Bot.Siml.Events;
using Synbot;
using System.Collections;
using EZ_B;

namespace SynbotPlugin
{
    public partial class Mainform : EZ_Builder.UCForms.FormPluginMaster
    {
        private Clienttcp.Clienttcp clienttcp = null;
        private Synbot.Synbot synbot;
        private string nomuser;
        private bool flagmessageerror = false;
        private string vrlanguageused = string.Empty;
        private static string variablebotusermessage = "$botusermessage";
        private static string variablebotevent = "$botevent";
        private static string variablebotvartype = "$botvartype";
        private static string variablebotvarname = "$botvarname";
        private static string variablebotvarvalue = "$botvarvalue";
        private static string variablebotuserid = "$botuserid";
        private static string variablebotstatus = "$botstarted";
        private static string defautbotname = "Maya";
        private static string packagename = "Testbot";
        private static bool flag_error_config = true;
        private static char messdelimiter = Convert.ToChar("|");
        private static char variabledelimiter = Convert.ToChar(":");
        private bool flagbotrunning = false;
        EZ_Builder.Scripting.Executor _executor_0;
        EZ_Builder.Scripting.Executor _executor_1;
        EZ_Builder.Scripting.Executor _executor_2;
        EZ_Builder.Scripting.Executor _executor_3;
        EZ_Builder.Scripting.Executor _executor_4;
        public Mainform()
        {
            InitializeComponent();
        }
        private void Mainform_Load(object sender, EventArgs e)
        {
            /// Create EZ Executors for script execution
            _executor_0 = new EZ_Builder.Scripting.Executor();
            _executor_1 = new EZ_Builder.Scripting.Executor();
            _executor_2 = new EZ_Builder.Scripting.Executor();
            _executor_3 = new EZ_Builder.Scripting.Executor();
            _executor_4 = new EZ_Builder.Scripting.Executor();
            // Set Input Userid to default userid stored in configuration
            EZ_Builder.Invokers.SetText(textBoxUserid, _cf.STORAGE[ConfigurationDictionary._STORAGE_USER_ID].ToString());
            //Create and initialize EZ variables used for Commandcontrol to Synbot
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotstatus, false.ToString());
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotevent, string.Empty);
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotvartype, string.Empty);
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotvarname, string.Empty);
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotvarvalue, string.Empty);
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotusermessage, string.Empty);
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotuserid, string.Empty);
            // Intercept all unknown functions called from any ez-script globally.
            // If a function is called that doesn't exist in the ez-script library, this event will execute
            ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent += FunctionEval_AdditionalFunctionEvent;
            
        }
        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {

            // Disconnect from the function event
            ExpressionEvaluation.FunctionEval.AdditionalFunctionEvent -= FunctionEval_AdditionalFunctionEvent;
            // free memory
            _executor_0 = null; _executor_1 = null; _executor_2 = null; _executor_3 = null; _executor_4 = null;
        }
        /// 
        /// This method is called by EZ-Builder when it requests the configuration for your  plugin. 
        /// EZ-Builder will request the configuration when the control is created and when the project is saved.
        /// The data set in this configuration will be serialized and saved in the EZ-Builder user's project.
        /// Custom data may be stored in the STORAGE dictionary.
        /// 
        public override EZ_Builder.Config.Sub.PluginV1 GetConfiguration()
        {
            return base.GetConfiguration();
        }
        /// 
        /// This method is called by EZ-Builder when a project is loaded or when the control is added to a project.
        /// Set your default data in here!
        /// The configuration from the user's project file will be set using this method.
        /// *Note: The  plugin must call Base.SetConfiguration(cf) in your override. See the example!
        /// Also note that this is a good place to ensure all required configuration data exists.
        /// In the case of a newer version of your  plugin where different configuration data may be required outside of the users configuration file, set it here.
        /// 
        public override void SetConfiguration(EZ_Builder.Config.Sub.PluginV1 cf)
        {
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_USER_ID, "Jean luc");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_PATH, @"C:\Testbot\source\current\");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_SAVE_PATH, @"C:\Testbot\");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_DEFAULT_ANSWER_MESSAGE, "Bot no response message");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_DEFAULT_LANGUAGE, "fr-FR");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_RESPONSE_VARIABLE, "$SynbotTextResponse");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_VALUE, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_XML, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_ROBOTTYPE, "JD");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_RESPONSE, true);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_EZB, false);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_FLAG_EXEC_BOTCOMMAND, true);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_SYNBOTSERVER_IPADRESS, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_SYNBOTSERVER_TCPPORT, "3001");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_VALUE, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_XML, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_VALUE, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_XML, string.Empty);
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_WELCOME_MESSAGE, "Welcome to Synbot");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_INIT_EVENT, "Bot init event");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_BEFORE_STOP_EVENT, "Bot stop event");
            cf.STORAGE.AddIfNotExist(ConfigurationDictionary._STORAGE_BOT_FLAG_FULLLOG, false);
            base.SetConfiguration(cf);
        }
        /// 
        /// This method is called by EZ-Builder when another control sends a command to this control using the EZ-Script ControlCommand() function.
        /// The available ControlCommand() functions for this control should be returned in the GetSupportedControlCommandsMethodsForSlave() override.
        /// 
        public override void SendCommand(string windowCommand, params string[] values)
        {
            string[] ptr = new string[3];
            string name = null;
            bool flagOK = true;
            int ind = 0;
            try
            {
                 if ((windowCommand.Equals("Query", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Start", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Stop", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Save", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Raiseevent", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Getvar", StringComparison.InvariantCultureIgnoreCase)) ||
                    (windowCommand.Equals("Setvar", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (values.Length != 0) throw new Exception(string.Format("no parameters expected .You passed {0}", values.Length));
                    if (windowCommand.Equals("Query", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotusermessage)) ptr[0] = string.Empty;
                        else ptr[0] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotusermessage);
                        if (ptr[0] == string.Empty) throw new Exception("Variable botusermessage is mandatory and cannot be empty");
                        name = "botquery";
                        ind = 1;
                    }
                    if (windowCommand.Equals("Start", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotuserid)) ptr[0] = string.Empty;
                        else ptr[0] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotuserid);
                        name = "botstart";
                        ind = 1;
                    }
                    if (windowCommand.Equals("Stop", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = "botstop";
                    }

                    if (windowCommand.Equals("Save", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = "botsave";
                    }
                    if (windowCommand.Equals("Raiseevent", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotevent)) ptr[0] = string.Empty;
                        else ptr[0] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotevent);
                        if (ptr[0] == string.Empty) throw new Exception("Variable botevent is mandatory and cannot be empty");
                        name = "raisebotevent";
                        ind = 1;
                    }
                    if (windowCommand.Equals("Getvar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotvarname)) ptr[0] = string.Empty;
                        else ptr[0] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotvarname);
                        if (ptr[0] == string.Empty) throw new Exception("Variable botvarname is mandatory and cannot be empty");
                        name = "getbotvar";
                        ind = 1;
                    }
                    if (windowCommand.Equals("Setvar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotvarname)) ptr[0] = string.Empty;
                        else ptr[0] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotvarname);
                        if (ptr[0] == string.Empty) throw new Exception("Variable botvarname is mandatory and cannot be empty");
                        if (!EZ_Builder.Scripting.VariableManager.DoesVariableExist(variablebotvarvalue)) ptr[1] = string.Empty;
                        else ptr[1] = EZ_Builder.Scripting.VariableManager.GetVariable(variablebotvarvalue);
                        if (ptr[1] == string.Empty) throw new Exception("Variable botvarvalue is mandatory and cannot be empty");
                        name = "setbotvar";
                        ind =2;
                    }
                    string[] arg = new string[ind];
                    if (ind != 0)
                    {
                        for (int i = 0; i < ind; i++)
                        {

                            arg[i] = ptr[i].Replace("\"" , "");
                        }
                    }
                    string result = CheckProcessfunctions(name, arg , out flagOK);
                    if (result == false.ToString()) throw new Exception("Bot is not running command cancelled");
                }
                else base.SendCommand(windowCommand, values);
            }
            catch (Exception ex)
            {

                EZ_Builder.EZBManager.Log("Error in control '{0}'. Message: {1}", this.Text, ex.Message);
            }
        }
        /// 
        /// This method is called by EZ-Builder for the CHEAT SHEET to receive all available ControlCommand() syntax for this control.
        /// When the ControlCommand() is called for this function, see the SendCommand() override.
        /// 
        public override object[] GetSupportedControlCommands()
        {
            List<object> cmds = new List<object>();
            cmds.Add("Query");
            cmds.Add("Start");
            cmds.Add("Stop");
            cmds.Add("Save");
            cmds.Add("Raiseevent");
            cmds.Add("Getvar");
            cmds.Add("Setvar");
            return cmds.ToArray();
        }
        private string Control_querybot(string[] ptr)
        {
            string response = null;
            if (ptr.Length != 1) throw new Exception(string.Format("1 parameters expected. You passed {0} and ptr {1}", ptr.Length , ptr[0]));
            else
            {
                if (flagbotrunning == false)
                    response = false.ToString();
                else response = Bot_Query(ptr[0].Replace("\"", ""));
            }
            return response;
        }
        private string Control_botraiseevent(string[] ptr)
        {
            bool flagOK = true;
            if (ptr.Length != 1) throw new Exception(string.Format("1 parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (flagbotrunning == false) throw new Exception("Bot is not running , you must start it");
                else flagOK = synbot.Bot_raiseevent(ptr[0]);
            }
            return string.Empty;
        }
        private string Control_getvarbot(string[] ptr)
        {
            string response = null;
            string type = null;
            string[] elem;
            if (ptr.Length != 1) throw new Exception(string.Format("1 parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (flagbotrunning == false) throw new Exception("Bot is not running , you must start it");
                else
                { 
                    elem = ptr[0].Split(variabledelimiter);
                    type = elem[0];
                    if (!type.Equals("var", StringComparison.InvariantCultureIgnoreCase) &&
                    !type.Equals("user", StringComparison.InvariantCultureIgnoreCase) &&
                    !type.Equals("bot", StringComparison.InvariantCultureIgnoreCase))
                        throw new Exception(string.Format("var or user or bot expected as first parameter -  You passed {0}", type));
                    else
                    { 
                        response = synbot.Bot_getvar(type , elem[1]);
                        EZ_Builder.Scripting.VariableManager.SetVariable(variablebotvarvalue, response);
                    }
                }
            }
            return response;
        }
        private string Control_setvarbot(string[] ptr)
        {
            bool flagOK = true;
            string type = null;
            string[] elem;
            if (ptr.Length != 2) throw new Exception(string.Format("2 parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (flagbotrunning == false) throw new Exception("Bot is not running , you must start it");
                else
                {
                    elem = ptr[0].Split(variabledelimiter);
                    type = elem[0];
                    if (!type.Equals("var", StringComparison.InvariantCultureIgnoreCase) &&
                    !type.Equals("user", StringComparison.InvariantCultureIgnoreCase) &&
                    !type.Equals("bot", StringComparison.InvariantCultureIgnoreCase))
                        throw new Exception(string.Format("var or user or bot expected as first parameter -  You passed {0}", type));
                    else flagOK = synbot.Bot_settvar(type, elem[1], ptr[1]);
                 }
            }
            return string.Empty;
        }
        private string Control_startbot(string[] ptr)
        {
            string flagOK = false.ToString();
            if (ptr.Length > 2) throw new Exception(string.Format("0 or 1 parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (flagbotrunning == true)
                { 
                    if (ptr.Length != 0 && nomuser != ptr[0] && ptr[0] != string.Empty)
                    {
                // If new bot start request with different userid - stop the bot and restart with new userid
                       Stop_bot();
                       nomuser = ptr[0];
                    }
                    else
                    {
                        flagOK = true.ToString();
                        return flagOK;
                    }
                }
                /// Start the bot with saved configuration user if no parameter userid sent
                else
                {
                    if (ptr.Length == 0 || ptr[0] == string.Empty) nomuser = _cf.STORAGE[ConfigurationDictionary._STORAGE_USER_ID].ToString();
                    else nomuser = ptr[0];
                    if (!Test_configuration_start_bot())
                    {
                        EZ_Builder.Invokers.SetAppendText(botConsole, true, "Error in configuration path and Package Simlk file fot the bot Verify");
                        throw new Exception("Error in configuration path and Package Simlk file");
                    }
                }
                flagOK = true.ToString();
                Bot_Start(nomuser);
                Set_buttons_bot_started();
            }
            return flagOK;
        }
        private string Control_stopbot(string[] ptr)
        {
            string flagOK = false.ToString();
            if (ptr.Length != 0) throw new Exception(string.Format("no parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (flagbotrunning == true) Stop_bot();
                flagOK = true.ToString();
            }
            return flagOK;
        }
        private string Control_savebot(string[] ptr)
        {
            string flagOK = false.ToString();
            if (ptr.Length != 0) throw new Exception(string.Format("no parameters expected. You passed {0}", ptr.Length));
            else
            {
                if (!flagbotrunning) flagOK = false.ToString();
                else
                {
                    // Save Bot Context Botsettings and Usersettings
                    synbot.SaveBot();
                    // Message saving OK to Bot Console
                    EZ_Builder.Invokers.SetAppendText(botConsole, true, "Saving Bot Context completed for user {0}", Synbot.Synbot.nameuser);
                    flagOK = true.ToString();
                }
            }
            return flagOK;
        }
        
        static string[] ToStringArray(object arg)
        {
            var collection = arg as System.Collections.IEnumerable;
            if (collection != null)
            {
                return collection
                  .Cast<object>()
                  .Select(x => x.ToString())
                  .ToArray();
            }
            if (arg == null)
            {
                return new string[] { };
            }

            return new string[] { arg.ToString() };
        }
        /// <summary>
        /// This is executed when a function is specified in any ez-scripting that isn't a native function.
        /// You can check to see if the function that was called is your function.
        /// If it is, do something and return something.
        /// If you don't return something, a default value of TRUE is returned.
        /// If you throw an exception, the ez-script control will receive the exception and present the error to the user.
        /// </summary>
        private void FunctionEval_AdditionalFunctionEvent(object sender, ExpressionEvaluation.AdditionalFunctionEventArgs e)
        {
            try
            {
                string[] arr = ToStringArray(e.Parameters); 
                string name = e.Name;
                bool flagOK;
                // Check if the function is one of our function 
                string result = CheckProcessfunctions(name, arr, out flagOK);
                if (result == false.ToString()) throw new Exception("Bot is not running command cancelled");
                e.ReturnValue = result;
            }
            catch (Exception ex)
            {
                EZ_Builder.EZBManager.Log("Error in control '{0}'. Message: {1}", this.Text, ex.Message);
            }
        }
        // Process message commands from TCP Client (Vocal recognition application)
        //
       
        private string CheckProcessfunctions (string name , string [] arr , out bool flag)
        {
            string returnvalue = string.Empty;
            flag = true;
            // Check if the function is one of our function 
            if (!name.Equals("botstart", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("botstop", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("botsave", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("botquery", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("setbotvar", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("getbotvar", StringComparison.InvariantCultureIgnoreCase) &&
                !name.Equals("raisebotevent", StringComparison.InvariantCultureIgnoreCase)
                )
                { 
                    flag = false;
                    return string.Empty;
                }
            if (name.Equals("setbotvar", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_setvarbot(arr);
            if (name.Equals("getbotvar", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_getvarbot(arr);
            if (name.Equals("botstart", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_startbot(arr);
            if (name.Equals("botstop", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_stopbot(arr);
            if (name.Equals("botsave", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_savebot(arr);
            if (name.Equals("botquery", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_querybot(arr);
            if (name.Equals("raisebotevent", StringComparison.InvariantCultureIgnoreCase)) returnvalue = Control_botraiseevent(arr);
            return (returnvalue);
        }
            // Executed when we received event from Synbot - Bot message received either as
            // a response to a query or in a raised event
            //
        string Messagereceived_processing(string message)
        {
            string EZMessage = string.Empty; string paramsay = string.Empty;string messtolog = string.Empty;
            ///If parameter script to be launched when we received a bot response
            ExecEZScript(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_RESPONSE_SCRIPT_VALUE].ToString());
            //Unstring bot Response to separate text response from EZB commands in several separate strings EZMessage and EZCmd []
            //Bot response can be formatted as textresponse%EZB Command1%EZBcommand2%... with the % separator delimiting the EZB Commands
            //
            // For DEBUG ONLY EZ_Builder.Invokers.SetAppendText(botConsole, true, "{0} : {1}", "DEBUG", message);
            if (message == string.Empty)
            {
                // If bot Emty response message is configured - send the corresponding message to the bot
                //
             // For DEBUG ONLY   EZ_Builder.Invokers.SetAppendText(botConsole, true, "{0}", "DEBUGEVENTNO");
                string mess1 = _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_DEFAULT_ANSWER_MESSAGE].ToString();
                if (mess1 != string.Empty)
                {
                    flagmessageerror = true;
                    Bot_Query(mess1);
                }

                //    if (event1 != string.Empty) synbot.Bot_raiseevent(event1);
                return string.Empty;
            }
            string[] botPart = message.Split('%');
            bool switchexecutor = false;
            // Purging the text message from bad caracters to avoid errors in EZ Say command
            StringBuilder sb = new StringBuilder(botPart[0].ToLower());
            sb.Replace("[", ""); sb.Replace("]", ""); sb.Replace("_", " "); sb.Replace('\t', ' ');
            sb.Replace('\n', ' '); sb.Replace('\r', ' '); sb.Replace("   ", " ");sb.Replace("  ", " ");
            EZMessage = sb.ToString();
            bool flagtosay = true;
            // If text message begin with | the message is not spoken but only displayed on Bot Console
            if (EZMessage != String.Empty)
            { 
                if (sb[0] == '|')
                { 
                    flagtosay = false;
                    EZMessage = EZMessage.Substring(1);
                }
            }
            // Set EZB variables
            // we need to test now content of messages and lauch cmd and sayEZB if included in options flag
            EZ_Builder.Scripting.VariableManager.SetVariable(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_RESPONSE_VARIABLE].ToString(), EZMessage);
            if (Convert.ToBoolean(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_FULLLOG])) messtolog = message;
            else messtolog = EZMessage;
            // Message to Bot Console
            EZ_Builder.Invokers.SetAppendText(botConsole, true, "{0} : {1}", Synbot.Synbot.namebot, messtolog);
            // test content of messages and lauch cmd and sayEZB if includeded in options flag
            if (flagtosay)
            { 
                if (Convert.ToBoolean(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_RESPONSE]) && EZMessage != "")
                {
                    paramsay = "(\"" + EZMessage + "\")";
                    if (Convert.ToBoolean(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_SPEAK_EZB])) paramsay = "SayEZB" + paramsay;
                    else paramsay = "Say" + paramsay;
                    _executor_4.StartScriptASync(paramsay);
                }
            }

            if (Convert.ToBoolean(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_FLAG_EXEC_BOTCOMMAND]) && botPart.Length > 1)
            {
                for (int i = 1; i < botPart.Length; i++)
                {
                    if (botPart[i] != "" && botPart[i] != null)
                    {
                        // 2 executors are used for executing the EZ commands in the bot response
                        // If more than 2 commands script executing will be stopped for executing the next one
                        EZ_Builder.Invokers.SetAppendText(botConsole, true, "Commande :{0}", botPart[i]);
                        if (switchexecutor)
                        {
                            _executor_2.StartScriptASync(botPart[i].Replace("[", "(").Replace("]", ")").Replace("|","\r\n"));
                            switchexecutor = true;
                        }
                        else
                        {
                            _executor_0.StartScriptASync(botPart[i].Replace("[", "(").Replace("]", ")").Replace("|", "\r\n"));
                switchexecutor = true;
                        }
                    }
                }
            }
            return messtolog;
        }   
        private void Bot_Start(string nomuser)
        {
            EZ_Builder.Invokers.SetText(textBoxUserid, nomuser);
            vrlanguageused = _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_DEFAULT_LANGUAGE].ToString();
            /// Create Synbot class instance
            synbot = new Synbot.Synbot(nomuser, _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_PATH].ToString(),
               packagename, _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_SAVE_PATH].ToString(),
                _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_ROBOTTYPE].ToString(), 
                _executor_0, defautbotname, vrlanguageused);
            /// event handler attached to the response message received from the Bot
            synbot.botUser.ResponseReceived += SimlBot_Responsereceived;
            /// Set EZB variables with userid Bot Name and User Name
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotuserid, nomuser);
            // Message initialize OK to Bot Console
            EZ_Builder.Invokers.SetAppendText(botConsole, true, "Bot initialized for user id {0}", nomuser);
            flagbotrunning = true;
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotstatus, true.ToString());
            //
            // If bot init event if configured - raise the corresponding bot event
            //
            string event1 = _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_INIT_EVENT].ToString();
            if (event1 != string.Empty) synbot.Bot_raiseevent(event1);
            ///If parameter script to be lauched just after bot Start
            ExecEZScript(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_START_SCRIPT_VALUE].ToString());
            //
            // Process welcom message if configured
            //
            string welcomemess = _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_WELCOME_MESSAGE].ToString();
            if (welcomemess != string.Empty)
            {
                Bot_Query(welcomemess);
            }
            return;
        }
        private void Bot_ReStart(string nomuser)
        {
            /// Create Synbot class instance
            synbot = new Synbot.Synbot(nomuser, _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_PATH].ToString(),
               packagename, _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_SAVE_PATH].ToString(),
                _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_ROBOTTYPE].ToString(),
                _executor_0, defautbotname, vrlanguageused);
            /// event handler attached to the response message received from the Bot
            synbot.botUser.ResponseReceived += SimlBot_Responsereceived;
            /// Set EZB variables with userid Bot Name and User Name
            return;
        }
        private string Bot_Query(string userphrase)
        {
            // Message to Bot Console with userphrase
            // Do some filtering not easy to do in Synbot
            StringBuilder sb = new StringBuilder(userphrase.ToLower());
            sb.Replace("-"," "); sb.Replace('\t', ' ');
            sb.Replace('\n', ' '); sb.Replace('\r', ' '); 
            string tempo = sb.ToString();
            if (!flagmessageerror) EZ_Builder.Invokers.SetAppendText(botConsole, true, "{0} : {1}", Synbot.Synbot.nameuser, tempo);
            flagmessageerror = false;
            // Query the bot with Userphrase
            string botMessage = synbot.BotQuery(tempo);
            // If error due to licensing problem Stop restart the bot and submit again
            if (botMessage == "ERRORERRORERROR")
            {
                EZ_Builder.Invokers.SetAppendText(botConsole, true, "{0} : {1}", Synbot.Synbot.nameuser, "Restarting the Bot ");
                synbot.Stopbot();
                synbot = null;
                // Bot_ReStart(nomuser);
                ///botMessage = Bot_Query(userphrase);
            }
            botMessage = Messagereceived_processing(botMessage);
            return botMessage; 
        }
         private void Stop_bot()
        {
            ///If parameter script to be lauched when we received a bot response
            ///cannot include command to the bot just event
            ExecEZScript(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_STOP_SCRIPT_VALUE].ToString());
            //
            // If bot before stop event if configured - raise the corresponding bot event
            //
            string event1 = _cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_BEFORE_STOP_EVENT].ToString();
            if (event1 != string.Empty) synbot.Bot_raiseevent(event1);
            /// Stop Bot
            synbot.Stopbot();
            synbot = null;
            Set_buttons_bot_stopped();
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotstatus, false.ToString());
            // clear the values of EZ variable user id bot name and username
            EZ_Builder.Scripting.VariableManager.SetVariable(variablebotuserid, string.Empty);
            flagbotrunning = false;
            // Message saving OK to Bot Console
            EZ_Builder.Invokers.SetAppendText(botConsole, true, "Saving Bot Context completed for user id {0}", nomuser);
        }
        public void ExecEZScript(string script)
        {
            if (script != "")
            {
                _executor_3.StartScriptASync(script);
                EZ_Builder.Invokers.SetAppendText(botConsole, true, "Script {0} started", script);
            }
            return;
        }
        public void SimlBot_Responsereceived(object sender, ResponseReceivedEventArgs e)
        {

            var chatResult = e.Result;
            string botMessage = chatResult.BotMessage;
            if (botMessage != "" && botMessage != null)
            {
                Messagereceived_processing(botMessage);
            }
        }
        private bool Test_configuration_start_bot()
        {
            flag_error_config = true;
            if (!Directory.Exists(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_PATH].ToString()))
                flag_error_config = false;
            if (!Directory.Exists(_cf.STORAGE[ConfigurationDictionary._STORAGE_BOT_SAVE_PATH].ToString()))
                flag_error_config = false;
            return flag_error_config;
        }
        private void Set_buttons_bot_stopped()
        {
            EZ_Builder.Invokers.SetText(button3, "Start Bot");
            EZ_Builder.Invokers.SetVisible(ucConfigurationButton1, true);
            EZ_Builder.Invokers.SetVisible(button1, false);
            EZ_Builder.Invokers.SetVisible(button2, false);
            EZ_Builder.Invokers.SetVisible(botInput, false);
            EZ_Builder.Invokers.SetEnabled(textBoxUserid, true);
        }
        private void Set_buttons_bot_started()
        {
            EZ_Builder.Invokers.SetText(button3, "Stop Bot");
            EZ_Builder.Invokers.SetVisible(ucConfigurationButton1, false);
            EZ_Builder.Invokers.SetVisible(button1, true);
            EZ_Builder.Invokers.SetVisible(button2, true);
            EZ_Builder.Invokers.SetVisible(botInput, true);
            EZ_Builder.Invokers.SetEnabled(textBoxUserid, false);
        }
        private void botConsole_TextChanged(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Save Bot Context Botsettings and Usersettings
            synbot.SaveBot();
            // Message saving OK to Bot Console
            EZ_Builder.Invokers.SetAppendText(botConsole, true, "Saving Bot Context completed for user id  {0}", nomuser);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // If BotInput entered - manage the input and Query the bot 
            if (botInput.Text != "") Bot_Query(botInput.Text);
            EZ_Builder.Invokers.SetText(botInput, "");
            EZ_Builder.Invokers.SetFocus(botInput);
        }

        private void ucConfigurationButton1_Click(object sender, EventArgs e)
        {
            using (ConfigurationForm form = new ConfigurationForm())
            {

                form.SetConfiguration(_cf);

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { 
                    SetConfiguration(form.GetConfiguration());
                    textBoxUserid.Text = _cf.STORAGE[ConfigurationDictionary._STORAGE_USER_ID].ToString();
                }     
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //Start and stop Bot
            if (!flagbotrunning)
            {
                //Start Bot
                if (!Test_configuration_start_bot())
                {
                    EZ_Builder.Invokers.SetAppendText(botConsole, true, "Error in configuration path and Package Simlk file fot the bot Verify");
                    return;
                }

                Set_buttons_bot_started();
                if (textBoxUserid.Text != string.Empty) nomuser = textBoxUserid.Text;
                else nomuser = _cf.STORAGE[ConfigurationDictionary._STORAGE_USER_ID].ToString();
                Bot_Start(nomuser);
                EZ_Builder.Invokers.SetFocus(botInput);
            }
            else
            {
                Stop_bot();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            EZ_Builder.Invokers.SetText(botConsole , "");
        }
        private void textBoxUserid_TextChanged(object sender, EventArgs e)
        {

        }
        private void botInput_KeyUp(object sender, KeyEventArgs e)
        {
        // TEst return Key - same effect as send button
            if (e.KeyCode == Keys.Return)
            {
                button1_Click(sender, e);
            }
        }
    }
}
