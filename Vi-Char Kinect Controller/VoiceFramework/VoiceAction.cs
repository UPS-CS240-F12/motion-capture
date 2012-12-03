using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceFramework
{
    public class VoiceAction
    {
        private string actionName;
        private string actionInput;

        public VoiceAction(string actionName, string actionInput)
        {
            this.actionName = actionName;
            this.actionInput = actionInput;
        }

        public bool IsMatch(string voiceInput)
        {
            return voiceInput.Equals(actionInput);
        }
    }
}
