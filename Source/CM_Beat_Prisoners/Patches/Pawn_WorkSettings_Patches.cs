using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners.Patches
{
    [StaticConstructorOnStartup]
    public static class Pawn_WorkSettings_Patches
    {
        [HarmonyPatch(typeof(Pawn_WorkSettings))]
        [HarmonyPatch("EnableAndInitialize", MethodType.Normal)]
        public static class Pawn_WorkSettings_EnableAndInitialize
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn_WorkSettings __instance, Pawn ___pawn, DefMap<WorkTypeDef, int> ___priorities)
            {
                if (___priorities != null)
                {
                    __instance.SetPriority(BeatPrisonersDefOf.CM_Beat_Prisoners_WorkType_Break_Resistance, 0);
                }
            }
        }
    }
}