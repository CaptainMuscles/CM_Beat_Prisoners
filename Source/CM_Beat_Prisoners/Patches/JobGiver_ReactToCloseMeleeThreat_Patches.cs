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
    public static class JobGiver_ReactToCloseMeleeThreat_Patches
    {
        [HarmonyPatch(typeof(JobGiver_ReactToCloseMeleeThreat))]
        [HarmonyPatch("TryGiveJob", MethodType.Normal)]
        public static class JobGiver_ReactToCloseMeleeThreat_TryGiveJob
        {
            [HarmonyPostfix]
            public static void Postfix(JobGiver_ReactToCloseMeleeThreat __instance, Pawn pawn, ref Job __result)
            {
                if (pawn.IsPrisoner && pawn.mindState.meleeThreat != null && __result != null)
                {
                    BeatingInProgress beating = Current.Game.World.GetComponent<BeatingTracker>()?.GetBeatingInProgress(pawn);

                    if (beating != null)
                    {
                        // If the perceived threat is not someone already beating us, then act as if they were
                        if (!beating.IsBeating(pawn.mindState.meleeThreat))
                        {
                            beating.AddBeater(pawn.mindState.meleeThreat);
                            beating.TryFightBack();
                        }
                        
                        // If we aren't fighting back, cower
                        if (!beating.fightingBack)
                        {
                            __result = JobMaker.MakeJob(BeatPrisonersDefOf.CM_Beat_Prisoners_Job_Cower, pawn.mindState.meleeThreat);
                        }
                    }
                }
            }
        }
    }
}