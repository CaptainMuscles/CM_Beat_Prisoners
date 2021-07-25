using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    public class WorkGiver_Warden_Break : WorkGiver_Warden
    {
        public override Job JobOnThing(Pawn pawn, Thing target, bool forced = false)
        {
            if (!ShouldTakeCareOfPrisoner(pawn, target, forced))
            {
                return null;
            }
            Pawn pawn2 = (Pawn)target;

            PrisonerInteractionModeDef interactionMode = pawn2.guest.interactionMode;
            BeatingTracker beatingTracker = Current.Game.World.GetComponent<BeatingTracker>();

            bool canGiveBeating = (beatingTracker?.CanGiveBeating(pawn) ?? true) && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            bool initiatorHealty = Mathf.Abs(1.0f - (pawn.health?.summaryHealth?.SummaryHealthPercent ?? 1.0f)) <= 0.01f;
            bool targetHealthy = Mathf.Abs(1.0f - (pawn2.health?.summaryHealth?.SummaryHealthPercent ?? 1.0f)) <= 0.01f;
            bool prisonerCanReceiveBeating = (interactionMode == PrisonerInteractionModeDefOf.ReduceResistance && !pawn2.Downed);
            
            if (canGiveBeating && prisonerCanReceiveBeating && initiatorHealty && (targetHealthy || beatingTracker.GetBeatingInProgress(pawn2) != null))
            {
                Job breakJob = JobMaker.MakeJob(BeatPrisonersDefOf.CM_Beat_Prisoners_Job_Break_Resistance, target);
                breakJob.maxNumMeleeAttacks = Rand.RangeInclusive(3, 9);
                return breakJob;
            }
            return null;
        }
    }
}
