using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CM_Beat_Prisoners
{
    public class BeatingTracker : WorldComponent
    {
        private const int minimumBeatingInterval = 30000;

        private List<BeatingInProgress> beatingsInProgress = new List<BeatingInProgress>();

        private List<BeatingCounter> beatingCounters = new List<BeatingCounter>();

        public BeatingTracker(World world) : base(world)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                int currentTick = Find.TickManager.TicksGame;

                beatingsInProgress = beatingsInProgress.Where(beating => beating.HasValidBeatee).ToList();
                beatingCounters = beatingCounters.Where(beating => beating.HasValidBeater && beating.nextBeatingTick > currentTick).ToList();
            }

            Scribe_Collections.Look(ref beatingsInProgress, "beatingsInProgress", LookMode.Deep);
            Scribe_Collections.Look(ref beatingCounters, "beatingCounters", LookMode.Deep);
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
        }

        public bool CanGiveBeating(Pawn beater)
        {
            BeatingCounter counter = beatingCounters.Find(bc => bc.beater == beater);

            if (counter == null)
                return true;

            int ticksUntilNextBeating = counter.nextBeatingTick - Find.TickManager.TicksGame;

            if (ticksUntilNextBeating > 0)
                Logger.MessageFormat(this, "{0} cannot give beating for another {1} ticks", beater, ticksUntilNextBeating);

            return (ticksUntilNextBeating <= 0);
        }

        public BeatingInProgress GetBeatingInProgress(Pawn beatee)
        {
            return beatingsInProgress.Find(bting => bting.beatee == beatee);
        }

        public BeatingInProgress GetOrStartBeatingInProgress(Pawn beatee, Pawn beater = null)
        {
            BeatingInProgress beating = GetBeatingInProgress(beatee);

            if (beating == null)
            {
                beating = new BeatingInProgress();
                beating.beatee = beatee;
                beatingsInProgress.Add(beating);

                Logger.MessageFormat(this, "Started a new beating for {0}", beatee);
            }

            if (beater != null)
            {
                beating.AddBeater(beater);
                BeatingCounter counter = beatingCounters.Find(bc => bc.beater == beater);
                if (counter == null)
                {
                    counter = new BeatingCounter();
                    counter.beater = beater;
                    counter.nextBeatingTick = Find.TickManager.TicksGame + minimumBeatingInterval;
                }
            }

            return beating;
        }

        public void BeaterDowned(Pawn beatee, Pawn beater)
        {
            GetBeatingInProgress(beatee)?.TryPrisonBreak();
        }

        public void StopBeating(Pawn beatee, Pawn beater)
        {
            BeatingInProgress beating = GetBeatingInProgress(beatee);

            if (beating != null)
            {
                beating.RemoveBeater(beater);
                if (beating.beaters.Count == 0)
                {
                    beatingsInProgress.Remove(beating);
                    Logger.MessageFormat(this, "Ended beating for {0}", beatee);
                }
            }
        }
    }
}
