using System.Collections.Generic;
using System.Text;

namespace StanceOverhaul.SpringAnimators
{
    internal class IdleAnimator : ISpringAnimator
    {
        //TODO: plays idle animations using its own spring instance
        //tracks when player is considered idle, using its own timers
        //handles checking whichever key inputs or actions should interrupt idle and reset timers
        //handles idle animations while injured, low stamina, stance-specific and no stance
    }
}
