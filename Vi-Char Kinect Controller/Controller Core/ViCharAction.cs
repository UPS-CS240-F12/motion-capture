using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller_Core
{
    public enum ViCharGesture
    {
        Moving, 
        TurningLeft,
        TurningRight, 
        Jumping,
        None
    }

    public enum ViCharVoiceAction
    {
        VoiceAttack,
        VoiceShield,
        VoiceNone
    }

    public class ViCharVoiceActionGrammar
    {
        public static List<string> Words = new List<string> { "pew", "shield" };
    }
}
