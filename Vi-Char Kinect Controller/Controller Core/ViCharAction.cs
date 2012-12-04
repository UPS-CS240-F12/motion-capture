using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller_Core
{
    public enum ViCharGesture
    {
        None,
        Moving, 
        TurningLeft,
        TurningRight, 
        Jumping
    }

    public enum ViCharVoiceAction
    {
        VoiceNone,
        VoiceAttack,
        VoiceShield
    }
}
