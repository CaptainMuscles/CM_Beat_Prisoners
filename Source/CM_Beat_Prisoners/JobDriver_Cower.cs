using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    public class JobDriver_Cower : JobDriver
    {
        protected Pawn Attacker => job.targetA.Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil toil = new Toil();
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 1200;
            toil.initAction = delegate
            {
                pawn.pather.StopDead();
            };
            toil.tickAction = delegate
            {
                if (pawn.IsHashIntervalTick(35))
                {
                    BeatingInProgress beating = Current.Game.World.GetComponent<BeatingTracker>()?.GetBeatingInProgress(pawn);
                    if (beating != null && beating.fightingBack)
                        EndJobWith(JobCondition.InterruptForced);
                }
            };
            yield return toil;
        }
    }
}