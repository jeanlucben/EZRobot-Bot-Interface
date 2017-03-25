///    Synbot Plugin Plugin for EZB Builder Software intefacing With Synthetic Intelligence Network SYNBOT Framework
///    Copyright(C) 2016  Jean-Luc BENARD
///    This program is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.
///    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
///    GNU General Public License for more details.
///    You should have received a copy of the GNU General Public License along with this program.If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Data;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Bot.Siml.Events;
using CreativeGurus.Weather.Wunderground;
using CreativeGurus.Weather.Wunderground.Models;

namespace Synbot
{
    public class Synbot
    {
        public static EZ_Builder.Scripting.Executor _executor_bot;
        public SimlBot simlBot;
        public BotUser botUser;
        public static string bot_name;
        public static string user_name;
        public static string path_bot;
        public static Synbot ref_bot;
        private string nomsave;
        private string useridbot;
        private string botsavepath;
        private const string listendseparator = "et ";
        private const string nomsavebot = "BotSettings.siml";
        private const string nomsavelearned = "Learned.siml";
        private const string nomsavememorized = "Memorized.siml";
        private const string botrobottype_variable = "Robottype";
        private const string botlanguage_variable = "Userlanguage";
        public const string mapnomfr = "genre_name_fr";
        public const string mapnomplurielfr = "name_pluriels_fr";
        public const string mapnomplurielreversedfr = "name_pluriels_reversed_fr";
        public static readonly string [] mapnomfravec = {"name_with_article_un_fr", "name_with_article_le_fr", "name_with_article_de_fr" };
        public static readonly string[] mapnomfravecreversed = { "name_with_article_un_reversed_fr", "name_with_article_le_reversed_fr", "name_with_article_de_reversed_fr" };

        public static string namebot
        {
            get { return bot_name;}
            set { bot_name = value; }
        }
        public static string nameuser
        {
            get { return user_name;}
            set { user_name = value;}
        }
        public static string pathbot
        {
            get { return path_bot; }
            set { path_bot = value; }
        }
        public static Synbot refbot
        {
            get { return ref_bot; }
            set { ref_bot = value; }
        }
        public Synbot(string userid , string botpath , string packagename , string botsavepath , string robottype 
         , EZ_Builder.Scripting.Executor  executor , string defautbotname , string language)
        {
            string packageString = "";
            string pathbotpk = botpath + packagename + ".simlpk";
            string pathbotsource = botpath;
            _executor_bot = executor;
            this.simlBot = new SimlBot();
            simlBot.Adapters.Add(new EzvarAdapter());
            simlBot.Adapters.Add(new EzcmdAdapter());
            simlBot.Adapters.Add(new CompareAdapter());
            simlBot.Adapters.Add(new VaropAdapter());
            simlBot.Adapters.Add(new LearnAdapter());
            Simladaptators.Simladaptators.adaptators.Add(this.simlBot);
            UserSIMLadaptators.Simladaptators.adaptators.Add(this.simlBot);
            this.botUser = simlBot.MainUser;
            //this.botUser = simlBot.CreateUser(userid);
            // Recreate package

            var elementList = new System.Collections.Generic.List<XDocument>();
            foreach (var simlFile in Directory.GetFiles(pathbotsource, "*.siml"))
            {
                var simlElement = XDocument.Load(simlFile);
                elementList.Add(simlElement);
            }
            string pathsettings = pathbotsource + @"\" + "Settings" + @"\";
            if (Directory.Exists(pathsettings))
            { 
                foreach (var simlFile in Directory.GetFiles(pathsettings))
                {
                    var simlElement = XDocument.Load(simlFile);
                    elementList.Add(simlElement);
                }
            }
            var packageref = simlBot.PackageManager.ConvertToPackage(elementList);
            File.WriteAllText(pathbotpk, packageref);

            /// Load SIML Package file
            var t = Task.Run(() => packageString = File.ReadAllText(pathbotpk));
            t.Wait();
            this.simlBot.PackageManager.LoadFromString(packageString);
            this.nomsave = userid + "-Settings.siml";
            this.useridbot = userid;
            this.botsavepath = botsavepath;
            refbot = this;
            path_bot = botpath;
            // attach event handler for the learning event of the bot
            this.simlBot.Learning += SimlBot_Learning;
            // attach event handler for the memorizing event of the bot
            this.simlBot.Memorizing += simlBot_Memorizing;
            Bot_Load(this.botsavepath);
            /// Extract Bot Name and User Name in settings
            bot_name = simlBot.Settings["Name"].Value;
            user_name = botUser.Settings["Name"].Value;
            /// Set Bot settings for robot type parameter
            botUser.Settings[botrobottype_variable].Value = robottype;
            /// Set Bot settings for language parameter
            botUser.Settings[botlanguage_variable].Value = language.Substring(0,2);
            if (user_name == null)
            {
                user_name = string.Empty;
                botUser.Settings["Name"].Value = user_name;
            }
            /// If Name is not initialized in  bot settings intialize it to detaut robot name 
            if (bot_name == null || bot_name == string.Empty)
            { 
                simlBot.Settings["Name"].Value = defautbotname;
                bot_name = defautbotname;
            }
            //Events to detect change in Bot Name and User Name
            //
            simlBot.Settings["Name"].Changed += (sender, args) =>
            {
                Synbot.bot_name = simlBot.Settings["Name"].Value;
            };
            botUser.Settings["Name"].Changed += (sender, args) =>
            {
                Synbot.user_name = botUser.Settings["Name"].Value;
            };
        }
        public string BotQuery(string userphrase)
        {
            var botMessage = string.Empty;
            var botvarresponse = string.Empty;
            ChatResult chatResult; 
            try
            {
                // Create chat request to Syn Bot - bot response is returned in botMessage 
                var chatRequest = new ChatRequest(userphrase, this.botUser);
                chatResult = simlBot.Chat(chatRequest);
                botMessage = chatResult.BotMessage;
                botvarresponse = botUser.Settings["bot_event_response"].Value;
                botMessage = botMessage + botvarresponse;
                if (botvarresponse != string.Empty)
                    botUser.Settings["bot_event_response"].Value = "";
            }
            catch (Exception ex)
            {
                // Need to close the Bot Restart and Retry
                botMessage = "ERRORERRORERROR";
                return botMessage;      
            }
            
            return botMessage;
        }
        public string Bot_getvar(string type , string namevar)
        {
            var botvarvalue = "";
            if (type.Equals("var", StringComparison.InvariantCultureIgnoreCase))
                botvarvalue = botUser.Vars[namevar].Value;
            if (type.Equals("user", StringComparison.InvariantCultureIgnoreCase))
                botvarvalue = botUser.Settings[namevar].Value; 
            if (type.Equals("bot", StringComparison.InvariantCultureIgnoreCase))
                botvarvalue = simlBot.Settings[namevar].Value;
            if (botvarvalue == null) botvarvalue = "";
        return botvarvalue;
        }
        public bool Bot_settvar(string type, string namevar , string value)
        {
            if (type.Equals("var", StringComparison.InvariantCultureIgnoreCase))
                botUser.Vars[namevar].Value = value;
            if (type.Equals("user", StringComparison.InvariantCultureIgnoreCase))                      
                botUser.Settings[namevar].Value = value;
            if (type.Equals("bot", StringComparison.InvariantCultureIgnoreCase))
                simlBot.Settings[namevar].Value = value;
             return true;
        }
        public bool Bot_raiseevent(string nameevent)
        {
            simlBot.Raise(nameevent, botUser);
            return true;
        }
        private void Bot_Settings_Load(string folder, string typesettings, string nomsave)
        {
            string filenewpath = folder + nomsave;
            if (File.Exists(filenewpath))
            {
                var xdoc = XElement.Load(filenewpath).Element(typesettings);
                if (xdoc != null)
                    if (typesettings == "UserSettings")
                        botUser.Settings.Load(xdoc);
                    else simlBot.Settings.Load(xdoc);
            }
        }
        private void Bot_Learned_Load(string folder, string typesettings, string savename)
        {
            string filenewpath;
            if (typesettings == "Learned")
                filenewpath = folder + savename;
            else
                filenewpath = folder + botUser.ID + "/" + savename;

            if (File.Exists(filenewpath))
            {
                var xdoc = XDocument.Load(filenewpath);
                if (typesettings == "Learned")
                    simlBot.AddSiml(xdoc);
                else
                    simlBot.AddSiml(xdoc, botUser);
            }
        }
        private void Bot_Load(string folder)
        {
            /// Load saved UserSettings for the botUser
            Bot_Settings_Load(folder, "UserSettings", nomsave);
            /// Load saved Botsettings
            Bot_Settings_Load(folder, "BotSettings", nomsavebot);
            Bot_Learned_Load(folder, "Learned", nomsavelearned);
            Bot_Learned_Load(folder, "Memorized", nomsavememorized);
        }
        /// event handler attached to the Learning event of the Bot 
        private void SimlBot_Learning(object sender, LearningEventArgs e)
        {
            e.Document.Save(botsavepath + nomsavelearned);
        }
        /// event handler attached to the Memorized event of the Bot 
        private void simlBot_Memorizing(object sender, MemorizingEventArgs e)
        {
            var filePath = e.User.ID + "/" + nomsavememorized;
            e.Document.Save(botsavepath + filePath);
        }
        public void SaveBot()
        {
            var simlMapsDocument = this.simlBot.Maps.GetDocument();
            string pathsettings = path_bot + @"\" + "Settings" + @"\";
        /// If Subdirectory settings is not existing Write Maps directly in the bot source Directory
            if (!Directory.Exists(pathsettings))
                pathsettings = path_bot + @"\";
            File.WriteAllText(pathsettings + "Maps.siml", simlMapsDocument.ToString());
            /// Save UserSettings for the botUser
            /// XDocument userSettingsDoc = this.simlBot.Users[useridbot].Settings.GetDocument();
            XDocument userSettingsDoc = this.simlBot.MainUser.Settings.GetDocument();
            userSettingsDoc.Save(botsavepath + nomsave);
            /// Save BotSettings 
            XDocument botSettingsDoc = this.simlBot.Settings.GetDocument();
            botSettingsDoc.Save(botsavepath + nomsavebot);
            return;
        }

        public void Stopbot()
        {
            // Save Bot Context Botsettings and Usersettings
            this.SaveBot();
            this.simlBot.Learning -= SimlBot_Learning;
            // detach event handler for the memorizing event of the bot
            this.simlBot.Memorizing -= simlBot_Memorizing;
            return;
        }

        // SIML Adaptators for extending the SIML language
        //
        public class WeatherAdapter : IAdapter
        // SIML Adaptator - Get and set EZ Builder variables 
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "Weather";
            // Syntax SIML
  

            public string Evaluate(Context parameter)
            {
                string cityname = ""; string cdeget = ""; string value = ""; string valvar = ""; bool flagok = true;
                if (parameter.Element.Attribute("City") != null) cityname = parameter.Element.Attribute("City").Value;
              
                if (parameter.Element.Attribute("Value") != null) value = parameter.Element.Attribute("Value").Value;
               
                // Get API Key in Bot User Settings 
                string key = parameter.Bot.Settings["Weatherundergound_APIkey"].Value;
                // Get Bot User language
                String querylanguage = parameter.User.Settings[botlanguage_variable].Value.ToUpper();
                WeatherClient client = new WeatherClient(key);
                client.GetForecast(QueryType.GlobalCity, new QueryOptions() { Country = "France", City = cityname ,  Language = querylanguage});

               









                return valvar;
            }
        }
        public class EzvarAdapter : IAdapter
        // SIML Adaptator - Get and set EZ Builder variables 
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "EZvar";
            // Syntax SIML
            // <x:EZvar Get = "nomvar" /> - get value of EZ variable nomvar
            // if variable doesn't exist return empty string
            // <x:EZvar Set = "nomvar" Value = "value" /> - set value of EZ variable nomvar to value(string)
            // nomvar is the name of EZ variable without $
            // for Set Synbot var errorEZ is set to True if OK and to False if EZ variable does not exist

            public string Evaluate(Context parameter)
            {
                string cdeset = ""; string cdeget = ""; string value = ""; string valvar = ""; bool flagok = true;
                if (parameter.Element.Attribute("Get") != null) cdeget = parameter.Element.Attribute("Get").Value;
                if (parameter.Element.Attribute("Set") != null) cdeset = parameter.Element.Attribute("Set").Value;
                if (parameter.Element.Attribute("Value") != null) value = parameter.Element.Attribute("Value").Value;
                if (cdeget == "" && cdeset == "") return ("Incorrect syntax");
                if (cdeget != "")
                {
                    if (EZ_Builder.Scripting.VariableManager.DoesVariableExist("$" + cdeget))
                        valvar = EZ_Builder.Scripting.VariableManager.GetVariable("$" + cdeget);
                    else
                    {
                        valvar = "";
                        flagok = false;
                    }
                }
                if (cdeset != "")
                { 
                    valvar = "";
                    EZ_Builder.Scripting.VariableManager.SetVariable("$" + cdeset, value);
                }
                parameter.User.Vars["errorEZ"].Value = flagok.ToString();
                return valvar;
            }
        }
        public class EzcmdAdapter : IAdapter
        // SIML Adaptator - Executes only one line of EZ-Script and blocks until it has completed.
        // This adaptator returns the result of the single line of code.
        //For example, if the code was a function to return the value of a servo (example GetServo(d0)),
        //the value of the servo d0 will be returned to bot SIML.
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "EZcmd";
            // Syntax SIML
            // <x:EZcmd "EZ script" />
            // 
            // 

            public string Evaluate(Context parameter)
            {
                string EZcommand = ""; string valvar = "";
                if (parameter.Element.Value != null) EZcommand = parameter.Element.Value;
                if (EZcommand == "") return ("Incorrect syntax");
                //For Debugging
                //EZ_Builder.EZBManager.Log("Command lauched by bot '{0}'", EZcommand);
                valvar = _executor_bot.ExecuteScriptSingleLine(EZcommand.Replace("[", "(").Replace("]", ")"));
                return valvar;
            }
        }
        public class CompareAdapter : IAdapter
        // SIML Adaptator - Compare two values in SIML - can be used to compare values of two variables
        // Return True is comparison is OK and False if not OK
        //
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "Compare";
            // Syntax SIML
            // <x:Compare>value1%operator%value2</x:Compare>
            // Operator can be equal or not equal
            // Return true or False - if incorrect syntax return False

            public string Evaluate(Context parameter)
            {
                string command = string.Empty;string result = false.ToString();
                if (parameter.Element.Value == null || parameter.Element.Value == string.Empty) return result;
                command = parameter.Element.Value;
                string[] cdePart = command.Split('%');

                if (cdePart.Length != 3) return result;
                if (cdePart[1].Equals("equal", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (cdePart[0] == cdePart[2]) result = true.ToString();
                }
                if (cdePart[1].Equals("not equal", StringComparison.InvariantCultureIgnoreCase)||
                    cdePart[1].Equals("notequal", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    if (cdePart[0] != cdePart[2]) result = true.ToString();
                }
                return result;
            }
        }
        public class VaropAdapter : IAdapter
        // SIML Adaptator - Compare two values in SIML - can be used to compare values of two variables
        // Return True is comparison is OK and False if not OK
        //
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "Varop";
            // Syntax SIML
            // <x:Varop>value1%operator%value2</x:Varop>
            // Operator can be + or -
            // Return the result of operation - if incorrect syntax return False

            public string Evaluate(Context parameter)
            {
                string command = string.Empty; string result = false.ToString();
                Int32 op1 = 0;Int32 op2 = 0;
                if (parameter.Element.Value == null || parameter.Element.Value == string.Empty) return result;
                command = parameter.Element.Value;
                string[] cdePart = command.Split('%');
                if (cdePart.Length != 3) return result;
                Int32.TryParse(cdePart[0], out op1);
                Int32.TryParse(cdePart[2], out op2);
                if (cdePart[1].Equals("+", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = (op1 + op2).ToString();
                }
                if (cdePart[1].Equals("-", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = (op1 - op2).ToString();
                }
                /// FOR DEBUG ONLY
                ///EZ_Builder.EZBManager.Log("DEBUG {0}:{1}:{2}:{3}", cdePart[0], cdePart[1], cdePart[2], result);
                return result;
            }
        }
        public class LearnAdapter : IAdapter
        // SIML Adaptator - Learn 
        {
            public bool IsRecursive => true;
            public XName TagName => SimlSpecification.Namespace.X + "Learn";
            public string Evaluate(Context parameter)
            {
                string cde = string.Empty; string valvar = string.Empty;
                string var1 = string.Empty; ; string var2 = string.Empty; ; string action = string.Empty; ;
                string flagpluriel = string.Empty;
                string[] valuemap1; string[] valuemap2;
                string name1; string genre1; string name2; string genre2;
                var listval = new List<String>();
                if (parameter.Element.Attribute("Get") != null)
                {
                    cde = parameter.Element.Attribute("Get").Value;
                    var1 = cde;
                    action = "Get";
                }
                if (parameter.Element.Attribute("List") != null)
                {
                    cde = parameter.Element.Attribute("List").Value;
                    var1 = cde;
                    flagpluriel = "0";
                    action = "List";
                }
                if (parameter.Element.Attribute("Set") != null)
                {
                    cde = parameter.Element.Attribute("Set").Value;
                    var1 = cde;
                    action = "Set";
                }
                if (parameter.Element.Attribute("Test") != null)
                {
                    cde = parameter.Element.Attribute("Test").Value;
                    var1 = cde;
                    action = "Test";
                }
                if (parameter.Element.Attribute("Value") != null)
                {
                    cde = parameter.Element.Attribute("Value").Value;
                    var2 = cde;
                }
                // uses only with Get or test not mandatory
                if (parameter.Element.Attribute("Flag") != null)
                {
                    flagpluriel = parameter.Element.Attribute("Flag").Value;
                }
                if (var1 == "") return ("Incorrect syntax");
                Decode_articlename_fr(var1, out name1, out genre1, out valuemap1);
                //EZ_Builder.EZBManager.Log("DEBUG {0}:{1}:{2}", name1, genre1, var1);
                Decode_articlename_fr(var2, out name2, out genre2, out valuemap2);
                //EZ_Builder.EZBManager.Log("DEBUG {0}:{1}:{2}", name2, genre2, var2);
                if (genre1 == "P")
                {
                    // Search in table of plural forms
                    var listemaps = parameter.Bot.Maps;
                    var mapref = listemaps[mapnomplurielfr];
                    name1 = mapref[name1].Value;
                }
                if (genre1 == "P" && name2 != string.Empty)
                {
                    // Seach in table of plural forms
                    var listemaps = parameter.Bot.Maps;
                    var mapref = listemaps[mapnomplurielfr];
                    name2 = mapref[name2].Value;
                }
                bool resultvar1 = false;
                bool resultvar2 = false;
                if (action == "Set" || action == "Test")
                { 
                    // test if first variable contains value for set and test
                    resultvar1 = Testifentryexist("fr_" + name1, "fr_" + name2, parameter);
                    // test if value variable contains reference to variable 1
                    resultvar2 = Testifentryexist("fr_list_" + name2, "fr_" + name1, parameter);
                }
                switch (action)
                {
                    case "Set":
                        {
                        Createmapentries(name1, genre1, valuemap1, parameter);
                        Createmapentries(name2, genre2, valuemap2, parameter);
                        // Add value to Bot User Settings Table
                        if (! resultvar1) parameter.User.Settings["fr_" + name1].Add("fr_" + name2);
                        if (! resultvar2) parameter.User.Settings["fr_list_" + name2].Add("fr_" + name1);
                        break;
                        }
                    case "Test":
                        var listpath = new List<String>();
                        if (!resultvar1)
                        {
                            var listvalue = new List<String>();
                            listvalue = parameter.User.Settings["fr_" + name1].List;
                            resultvar1 = Testdeepifentryexist(listvalue, listpath,  "fr_" + name2, parameter);
                        }
                        valvar = resultvar1.ToString();
                        // ability to set a bot variable with a test corresponding to the path
                        // but same as list
                        break;
                    case "Get":
                    case "List":
                        // need to send in response count an to set synbot var with response elements
                        // synbot response var are named Varresult_1, Varresult_1 etc

                        var listemaps = parameter.Bot.Maps;
                        var mapref1 = listemaps[mapnomplurielreversedfr];
                        var mapref = listemaps[mapnomfravecreversed[0]];
                        int count = 0;
                        string reftopvariable = string.Empty;
                        if (action == "List")
                        {
                            reftopvariable = "fr_list_" + name1;
                        // if Plural form response with article Le or La instead of un une
                            mapref = listemaps[mapnomfravecreversed[1]];
                        }
                        else
                        {
                            reftopvariable = "fr_" + name1;
                        }
                       
                        count = parameter.User.Settings[reftopvariable].Count;
                        //Varresult Bot variable is set with the name
                        parameter.User.Vars["Varresult"].Value = name1;
                        valvar = count.ToString();
                        if (count != 0)
                        {
                            listval = parameter.User.Settings[reftopvariable].List;
                            if (action != "List")
                            {
                                // Get list od direct references
                                for (int i = 0; i < listval.Count; i++)
                                {
                                    var listresult = new List<String>();
                                    // take Fr_name and serach in reverse Un Name map
                                    if (flagpluriel != "O")
                                    { 
                                        listresult.Add(mapref[listval[i]].Value);
                                        Treesearch(listval[i], parameter, ref listresult, mapref , "");
                                    }
                                    // Take fr_name suppress fr_ and search in reverse plural map the plural form concatenated with Des article
                                    else
                                    { 
                                        listresult.Add("Des " + mapref1[listval[i].Substring(3)]);
                                        Treesearch(listval[i], parameter, ref listresult, mapref1, "O");
                                    }
                                    
                                    parameter.User.Vars["Varresult_" + (i+1).ToString()].Value = Createlibfromlist(ref listresult);
                                }
                            }
                            else
                            {
                                var listresult = new List<String>();
                                for (int i = 0; i < listval.Count; i++)
                                {
                                    EZ_Builder.EZBManager.Log("DEBUG1 {0}:{1}:{2}:{3}", count, i, listval[i], "");
                                    listresult.Add(mapref[listval[i]].Value);
                                }
                                parameter.User.Vars["Varresult_1"].Value = Createlibfromlist(ref listresult);
                            }
                        }
                        break;
                    default:
                        break;
                }
                return valvar;
            }
            public string Createlibfromlist(ref List<String> resulttree)
            {
                string result = string.Empty;
                for (int i = 0; i < resulttree.Count; i++)
                {
                    if (i == resulttree.Count - 1 && i != 0) result = result + listendseparator;
                    result = result + resulttree[i] + " ";
                }
                return result;
            }
            public bool Treesearch(string name, Context parameter , ref List<String> resulttree , IMap mapref , string flagplural)
            {
                var listval = parameter.User.Settings[name].List;
                foreach (string varvalue in listval)
                {
                    if (flagplural != "O") resulttree.Add(mapref[varvalue].Value);
                    else resulttree.Add("Des " + mapref[varvalue.Substring(3)]);
                    Treesearch(varvalue, parameter, ref resulttree, mapref, flagplural);
                }
                return true;
            }
            public bool Testifentryexist(string namesource,string namesearched, Context parameter)
            {
                var refvar = parameter.User.Settings[namesource];
                return(refvar.Contains(namesearched));
            }
            public bool Testdeepifentryexist(List<String> listsource, List<String> listpath, string namesearched, Context parameter)
            {
                bool valuefound = false;
                foreach (string varvalue in listsource)
                {
                    valuefound = Testifentryexist(varvalue, namesearched, parameter);
                    if (valuefound)
                    {
                        listpath.Add(varvalue);
                        break;
                    } 
                    else
                    {
                        var listvalue = new List<String>();
                        listvalue = parameter.User.Settings[varvalue].List;
                        valuefound = Testdeepifentryexist(listvalue, listpath, namesearched, parameter);
                    }
                 }
                return valuefound;
            }
            public void Createmapentries(string name, string genre, string [] valuemap,Context parameter)
            {                         
                // Get bot mapcollection
                var listemaps = parameter.Bot.Maps;
                var mapref = listemaps[mapnomfr];
                var mapref1 = listemaps[mapnomfr];
                if (!mapref.Contains(name))
                {
                    mapref.Add(name, genre);
                    for (int i = 0; i < valuemap.Length; i++)
                    {
                        if (valuemap[i] != string.Empty)
                        {
                            mapref1 = listemaps[mapnomfravec[i]];
                            mapref1.Add(valuemap[i], "fr_" + name);
                            mapref1 = listemaps[mapnomfravecreversed[i]];
                            mapref1.Add("fr_" + name , valuemap[i]);
                        }
                    }
                    if (genre != "N")
                        /// Add reference to plural form of name
                    {
                        mapref = listemaps[mapnomplurielfr];
                        string pluralname = Pluriel_name_fr(name);
                        mapref.Add(pluralname, name);
                        mapref = listemaps[mapnomplurielreversedfr];
                        mapref.Add(name , pluralname);
                    }
                }
            }

            // Take the input and return name  to search and genre M F N(eutral) or P(lural)
            public void Decode_articlename_fr(string articlename, out string name, out string genre, out string[] var1)
            {
                name = string.Empty;
                genre = string.Empty;
                var1 = new string[3] { string.Empty, string.Empty, string.Empty };
                if (articlename == string.Empty) return;
                string firstlettername = string.Empty;
                string art1 = string.Empty; string art2 = string.Empty;
                articlename = articlename.ToLower();
                bool OKun = articlename.StartsWith("un ");
                bool OKune = articlename.StartsWith("une ");
                bool OKla = articlename.StartsWith("la ");
                bool OKle = articlename.StartsWith("le ");
                bool OKdes = articlename.StartsWith("des ");
                bool OKde = articlename.StartsWith("de ");
                bool OKdu = articlename.StartsWith("du ");
                bool OKdela = articlename.StartsWith("de la ");
                bool OKdel = articlename.StartsWith("de l'");
                bool OKdudela = (OKde || OKdu || OKdela || OKdel);
                if (!(OKun || OKune || OKla || OKle || OKdes || OKdudela))
                {
                    name = articlename;
                    var1[0] = name;
                    var1[1] = name;
                    var1[2] = "de " + name;
                    genre = "N";
                }
                else
                {
                    if (!OKdes && !OKdudela)
                    { 
                        if (OKune) name = articlename.Substring(4);
                        else name = articlename.Substring(3);
                        if (OKun || OKle)
                        {
                            art1 = "le ";
                            art2 = "du ";
                            genre = "M";
                        }
                        else
                        {
                            art1 = "la ";
                            art2 = "de la ";
                            genre = "F";
                        }
                        firstlettername = name.Substring(0, 1);
                        if (firstlettername == "a" || firstlettername == "e" || firstlettername == "i" ||
                            firstlettername == "o" || firstlettername == "u" || firstlettername == "y" ||
                            firstlettername == "é" || firstlettername == "è" || firstlettername == "ê" ||
                            firstlettername == "à" || firstlettername == "ô" || firstlettername == "û")
                        {
                            art1 = "l'";
                            art2 = "de l'";
                        }
                        if (OKun || OKune)
                        {
                            var1[0] = articlename;
                            var1[1] = art1 + name;
                            var1[2] = art2 + name;
                        }
                        else
                        {
                            var1[1] = articlename;
                            var1[2] = art2 + name;
                        }
                    }
                    else
                    { 
                        if (OKdes)
                        //Case plural form set genre to "P" var1 to empty
                        // Plural form ist just used with Test action
                        {
                            name = articlename.Substring(4);
                            genre = "P";
                        }
                        else
                        //Case input = de de la or d' - used only with Get action
                        {
                            int index = 3;
                            if (OKdela) index = 6;
                            if (OKdel) index = 5;
                            name = articlename.Substring(index);
                            genre = "G";
                        }
                    }
                }
                return;
            }
            /// <summary>
            /// French ortograph rules to get plural form of a name
            /// </summary>
            /// <param name="name"></param>
            /// <param name="plurielname"></param>
            public string Pluriel_name_fr(string name)
            {
                string[] pluralexceptionau = { "landau", "sarrau", "bleu", "pneu" };
                string[] pluralexceptionou = { "bijou", "caillou", "chou", "genou", "hibou", "joujou", "pou" };
                string[] pluralexceptionail = { "bail", "corail", "émail", "soupirail", "travail", "vantail", "vitrail" };
                string[] pluralexceptional = { "bal", "carnaval", "chacal", "festival", "récital", "régal" };
                List<string> exceptionau = new List<string>(pluralexceptionau);
                List<string> exceptionou = new List<string>(pluralexceptionou);
                List<string> exceptionail = new List<string>(pluralexceptionail);
                List<string> exceptional = new List<string>(pluralexceptional);
                name = name.ToLower();
                // Simplification if name contains several words - only the first will be set in plural form
                string[] namepart = name.Split(' ');
                String firstword = namepart[0];
                int lenfirstword = firstword.Length;
                string plurielname = firstword + "s";
                bool OKszx = (firstword.EndsWith("s") || firstword.EndsWith("z") || firstword.EndsWith("x"));
                bool OKau = (firstword.EndsWith("au") || firstword.EndsWith("eau") || firstword.EndsWith("eu"));
                bool OKou = (firstword.EndsWith("ou"));
                bool OKail = (firstword.EndsWith("ail"));
                bool OKal = (firstword. EndsWith("al"));
                if (OKszx) plurielname = firstword;
                if (OKau)
                {
                   if (! exceptionau.Contains(firstword)) plurielname = firstword + "x";
                }
                if (OKou)
                {
                    if (exceptionou.Contains(firstword)) plurielname = firstword + "x";
                }
                if (OKail)
                {
                    if (exceptionail.Contains(firstword))
                    {
                        string prefix = firstword.Substring(0, lenfirstword - 3);
                        plurielname = prefix + "aux";
                    }
                }
                if (OKal)
                {
                    if (!exceptional.Contains(firstword))
                    {
                        string prefix = firstword.Substring(0, lenfirstword - 2);
                        plurielname = prefix + "aux";
                    }
                }
                namepart[0] = plurielname;
                string result = string.Empty;
                foreach (string wordvalue in namepart)
                {
                    result = result + wordvalue + " ";
                }
                return result.Trim();
            }
        }
     }
}
