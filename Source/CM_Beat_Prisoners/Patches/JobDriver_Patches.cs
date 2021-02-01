using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    [StaticConstructorOnStartup]
    public static class JobDriver_Patches
    {
        [HarmonyPatch(typeof(JobDriver))]
        [HarmonyPatch("Cleanup", MethodType.Normal)]
        public static class JobDriver_Cleanup
        {
            [HarmonyPostfix]
            public static void Postfix(JobDriver __instance, JobCondition condition)
            {
                JobDriver_Break jobDriverBreak = __instance as JobDriver_Break;
                if (jobDriverBreak != null)
                {
                    Pawn initiator = jobDriverBreak.pawn;
                    BeatingTracker beatingTracker = Current.Game.World.GetComponent<BeatingTracker>();

                    if (initiator.Downed || initiator.Dead)
                    {
                        // If this became a fight, losing might trigger a prison break
                        beatingTracker?.BeaterDowned(jobDriverBreak.Victim, initiator);
                    }

                    beatingTracker?.StopBeating(jobDriverBreak.Victim, initiator);
                }
            }
        }
    }
}