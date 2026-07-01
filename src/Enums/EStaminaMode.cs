namespace StanceOverhaul.Enums;

public enum EStaminaMode
{
    Neutral, // do nothing — game handles HandsStamina fully
    Regen,   // add rate * dt to Current each frame (clamped to TotalCapacity)
    Drain,   // subtract rate * dt from Current each frame (clamped to 0)
    Freeze   // write DisableRestoration = Time.time + 1f each frame — suppresses SelfRestoration
             // while leaving all game drain consumptions (vault, melee, aim) unaffected
}
