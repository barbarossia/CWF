using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{


    public class EditEventArgs : System.EventArgs
    {
        EditReturnValue _editReturnValue = new EditReturnValue();

        public EditEventArgs(EditReturnValue returnValue)
        {
            _editReturnValue = returnValue; 
        }

        public EditReturnValue ReturnValue()
        {
            return _editReturnValue;
        }
    }

    [DataContract]
    public class EditReturnValue
    {
        bool _IsWarning = false;
        [DataMember]
        public bool IsWarning 
        {
            get { return _IsWarning; }
            set { _IsWarning = value; } 
        }

        [IgnoreDataMember]
        public bool IsError
        {
            get { return !IsValid; }
            set { IsValid = !value; }
        }

        [DataMember]
        public bool IsValid { get; set; }
        [DataMember]
        public IList<String> Messages { get; set; }

        public EditReturnValue()
        {
            IsValid = true;
            Messages = new List<string>();

        }

        public EditReturnValue(bool validFlag, string message) : this()
        {
            IsValid = validFlag;
            Messages.Add(message);
        }

        public EditReturnValue(bool validFlag, IList<string> messages)
        {
            IsValid = validFlag;
            Messages = messages;
        }
        /// <summary>
        /// Adds a message without changing the IsValid flag.
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Adds a message and updates the flag only if the IsValid
        /// value is already true.
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="message"></param>
        public void AddMessage(bool flag, string message)
        {
            if (IsValid)
                IsValid = flag;
            Messages.Add(message);
        }

        //
        /// <summary>
        /// Gets the message list as a single string with linefeeds seperating the messages
        /// </summary>
        /// <returns></returns>
        public string DisplayString()
        {
            string msg = "";
            foreach (var item in Messages)
            {
                msg = string.Concat(msg, item, "\r\n");
            }
            return msg;
        }
    }

}
