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

    public class ViCharConversion
    {
        public static Byte ActionToByte(ViCharVoiceAction type)
        {
            switch (type)
            {
                case ViCharVoiceAction.VoiceAttack: return 5;
                case ViCharVoiceAction.VoiceShield: return 6;
                default: return 0;
            }
        }

        public static Byte GestureToByte(ViCharGesture type)
        {
            switch (type)
            {
                case ViCharGesture.Moving: return 1;
                case ViCharGesture.TurningLeft: return 2;
                case ViCharGesture.TurningRight: return 3;
                case ViCharGesture.Jumping: return 4;
                default: return 0;
            }
        }
    }
}
