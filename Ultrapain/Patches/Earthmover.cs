using System;
using System.Collections.Generic;
using System.Text;

namespace Ultrapain.Patches
{
    class CountdownLengthPatch
    {
        static bool Prefix(Countdown __instance, ref float __result)
        {
            if (__instance.difficulty >= 6)
            {
                __result = 50f;
                return false;
            }
            return true;
        }
    }
}
