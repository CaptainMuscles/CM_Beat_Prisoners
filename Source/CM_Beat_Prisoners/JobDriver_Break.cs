using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    public class JobDriver_Break : JobDriver
    {
        private int numMeleeAttacksMade;

        public Pawn Victim => job.targetA.Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
            //return pawn.Reserve(Victim, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil beatingContinues = Toils_General.Label();
            Toil beatingComplete = Toils_General.Label();
            Toil beatingCancelled = Toils_General.Label();

            yield return Toils_General.Do(delegate
            {
                Messages.Message("CM_Beat_Prisoners_Break_Attempt".Translate(pawn, Victim), Victim, MessageTypeDefOf.NeutralEvent);
            });

            yield return beatingContinues;

            yield return GotoPrisoner(pawn, Victim);
            yield return Toils_Interpersonal.GotoInteractablePosition(TargetIndex.A);
            yield return ThreatenPrisoner(pawn, Victim);

            yield return Toils_General.Do(delegate
            {
                BeatingInProgress beating = Current.Game.World.GetComponent<BeatingTracker>()?.GetOrStartBeatingInProgress(Victim, pawn);
                if (beating != null)
                    beating.TryFightBack();
            });

            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
            yield return Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, delegate
            {
                if (Victim == null || !Victim.Spawned || Victim.InMentalState || !Victim.IsPrisonerOfColony || !Victim.guest.PrisonerIsSecure || Victim.guest.interactionMode != PrisonerInteractionModeDefOf.ReduceResistance)
                    pawn.jobs.curDriver.JumpToToil(beatingCancelled);

                if (Victim.Downed)
                    pawn.jobs.curDriver.JumpToToil(beatingComplete);

                if (pawn.meleeVerbs.TryMeleeAttack(Victim, job.verbToUse) && pawn.CurJob != null && pawn.jobs.curDriver == this)
                {
                    numMeleeAttacksMade++;

                    BeatingInProgress beating = Current.Game.World.GetComponent<BeatingTracker>()?.GetBeatingInProgress(Victim);
                    // Keep going if there is a fight, otherwise do the requested number of attacks
                    if ((beating != null && beating.fightingBack) || numMeleeAttacksMade < job.maxNumMeleeAttacks)
                    {
                        pawn.jobs.curDriver.JumpToToil(beatingContinues);
                    }
                    else
                    {
                        pawn.jobs.curDriver.JumpToToil(beatingComplete);
                    }
                }
            });

            yield return beatingComplete;

            Toil giveThoughts = Toils_General.Do(delegate
            {
                pawn.records.Increment(BeatPrisonersDefOf.CM_Beat_Prisoners_Record_Prisoners_Beaten);
                BeatPrisonersUtility.GiveThoughtsForPrisonerBeaten(Victim, pawn);

                // This never seems to get called, because if the initiator goes down, the job is interrupted
                //  Leaving it anyway to show desired behavior. This needs to go in JobDriver_Patches for Cleanup
                if (pawn.Downed || pawn.Dead)
                {
                    // If this became a fight, losing might trigger a prison break
                    Current.Game.World.GetComponent<BeatingTracker>()?.BeaterDowned(Victim, pawn);
                    pawn.jobs.curDriver.JumpToToil(beatingCancelled);
                }
            });
            yield return giveThoughts;

            //yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
            yield return BreakResistance(TargetIndex.A);

            yield return beatingCancelled;

            yield return Toils_General.Do(delegate
            {
                Current.Game.World.GetComponent<BeatingTracker>()?.StopBeating(Victim, pawn);
            });   
        }

        private Toil GotoPrisoner(Pawn pawn, Pawn talkee)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                pawn.pather.StartPath(talkee, PathEndMode.Touch);
            };
            toil.AddFailCondition(delegate
            {
                if (talkee.DestroyedOrNull())
                {
                    return true;
                }
                if (!talkee.IsPrisonerOfColony)
                {
                    return true;
                }
                return (talkee.guest == null) ? true : false;
            });
            toil.socialMode = RandomSocialMode.Off;
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        private Toil ThreatenPrisoner(Pawn pawn, Pawn talkee)
        {
            Toil toil = new Toil
            {
                initAction = delegate
                {
                    pawn.interactions.TryInteractWith(talkee, BeatPrisonersDefOf.CM_Beat_Prisoners_Interaction_Prisoner_Threatened);
                }
            };
            //toil.FailOn(() => !talkee.guest.ScheduledForInteraction);
            toil.socialMode = RandomSocialMode.Off;
            // This needs to be instant in case a fight is progress
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            //toil.defaultCompleteMode = ToilCompleteMode.Delay;
            //toil.defaultDuration = 175;
            toil.activeSkill = () => SkillDefOf.Social;
            return toil;
        }

        private Toil BreakResistance(TargetIndex recruiteeInd)
        {
            Toil toil = new Toil();
            toil.AddFinishAction(delegate
            {
                Pawn actor = toil.actor;
                Pawn pawn = (Pawn)actor.jobs.curJob.GetTarget(recruiteeInd).Thing;
                if (pawn.Spawned)// && pawn.Awake())
                {
                    InteractionDef intDef = BeatPrisonersDefOf.CM_Beat_Prisoners_Interaction_Prisoner_Beating_Conclusion;
                    actor.interactions.TryInteractWith(pawn, intDef);
                }
            });
            toil.socialMode = RandomSocialMode.Off;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 175;
            toil.activeSkill = () => SkillDefOf.Social;
            return toil;
        }
    }
}