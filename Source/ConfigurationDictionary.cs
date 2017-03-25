using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynbotPlugin
{
    class ConfigurationDictionary
    {
        public static readonly string _STORAGE_USER_ID = "user id";
        public static readonly string _STORAGE_BOT_PATH = "Folfer path for Bot file Simlpk";
        public static readonly string _STORAGE_BOT_SAVE_PATH = "Folder path for Bot and user Settings and learned memorized files";
        public static readonly string _STORAGE_BOT_DEFAULT_ANSWER_MESSAGE = "Bot message sent in case of no successful Bot answer";
        public static readonly string _STORAGE_BOT_DEFAULT_LANGUAGE = "Default culture used fr-FR , ....";
        public static readonly string _STORAGE_BOT_RESPONSE_VARIABLE = "EZ variable for storing text bot response";
        public static readonly string _STORAGE_BOT_RESPONSE_SCRIPT_VALUE = "EZ script value to execute when bot response received";
        public static readonly string _STORAGE_BOT_RESPONSE_SCRIPT_XML = "EZ script XML to execute when bot response received";
        public static readonly string _STORAGE_BOT_ROBOTTYPE = "Robot TYPE To test in SIML variable Robottype";
        public static readonly string _STORAGE_BOT_FLAG_SPEAK_RESPONSE = "Flag for speaking or not Bot text response";
        public static readonly string _STORAGE_BOT_FLAG_SPEAK_EZB = "Flag for speaking to EZB-V4";
        public static readonly string _STORAGE_BOT_FLAG_EXEC_BOTCOMMAND = "Flag for executing or not Bot command response if any";
        public static readonly string _STORAGE_SYNBOTSERVER_IPADRESS = "Synbot server IP adress ex 192.168.1.15";
        public static readonly string _STORAGE_SYNBOTSERVER_TCPPORT = "Synbot TCP server port";
        public static readonly string _STORAGE_BOT_START_SCRIPT_VALUE = "EZ script value to execute when bot start";
        public static readonly string _STORAGE_BOT_START_SCRIPT_XML = "EZ script XML to execute when bot start";
        public static readonly string _STORAGE_BOT_STOP_SCRIPT_VALUE = "EZ script value to execute when bot stop";
        public static readonly string _STORAGE_BOT_STOP_SCRIPT_XML = "EZ script XML to execute when bot stop";
        public static readonly string _STORAGE_BOT_WELCOME_MESSAGE = "Message send to Bot when welcoming after bot starting";
        public static readonly string _STORAGE_BOT_INIT_EVENT = "Bot event raised after bot starting";
        public static readonly string _STORAGE_BOT_BEFORE_STOP_EVENT = "Bot event raised just before bot stopping";
        public static readonly string _STORAGE_BOT_FLAG_FULLLOG = "Flag for displaying or not full bot response including EZ command if specified in SIML response";
    }
}
