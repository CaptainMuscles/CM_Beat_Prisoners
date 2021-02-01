using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CM_Beat_Prisoners
{
    public class BeatingInProgress : IExposable
    {
        public Pawn beatee = null;
        public List<Pawn> beaters = new List<Pawn>();

        public bool fightingBack = false;

        public float startingPainLevel = 0.0f;

        public bool HasValidBeatee => beatee != null &&
                                      beatee.Spawned &&
                                      beatee.IsPrisonerOfColony &&
                                      beatee.IsPrisonerInPrisonCell();

        private const float baseFightBackChance = 0.05f;
        private const float fightBackChanceMeleeFactor = 0.02f;

        private const float basePrisonBreakChance = 0.25f;

        private static List<Pair<string, float>> fightBackTraitFactors = new List<Pair<string, float>> { new Pair<string, float> ("Wimp", 0.5f),
                                                                                                         new Pair<string, float> ("Kind", 0.5f),
                                                                                                         new Pair<string, float> ("Masochist", 0.0f),
                                                                                                         new Pair<string, float> ("Brawler", 2.0f),
                                                                                                         new Pair<string, float> ("Bloodlust", 2.0f) };

        public BeatingInProgress()
        {
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref beatee, "beatee");
            Scribe_Collections.Look(ref beaters, "beaters", LookMode.Reference);

            Scribe_Values.Look(ref fightingBack, "fightingBack", false);
            Scribe_Values.Look(ref startingPainLevel, "startingPainLevel", 0.0f);
        }

        public float GetAndResetPainInflicted(float currentPain)
        {
            float painInflicted = Mathf.Max(0.0f, currentPain - startingPainLevel);
            startingPainLevel = currentPain;

            return painInflicted;
        }

        public void AddBeater(Pawn newBeater)
        {
            if (!beaters.Contains(newBeater))
            {
                beaters.Add(newBeater);
                Logger.MessageFormat(this, "{0} joined in on {1}'s beating", newBeater, beatee);
            }
        }

        public void RemoveBeater(Pawn oldBeater)
        {
            if (beaters.Contains(oldBeater))
            {
                beaters.Remove(oldBeater);
                Logger.MessageFormat(this, "{0} no longer beating {1}", oldBeater, beatee);
            }
        }

        public bool IsBeating(Pawn beater)
        {
            return beaters.Any(bter => bter == beater);
        }

        public void TryFightBack()
        {
            if (!HasValidBeatee)
            {
                fightingBack = false;
                return;
            }

            if (!fightingBack)
            {
                float fightBackChance = baseFightBackChance;

                if (beatee.story != null && beatee.story.traits != null && beatee.skills != null)
                {
                    fightBackChance += fightBackChanceMeleeFactor * beatee.skills.GetSkill(SkillDefOf.Melee).Level;
                    fightBackChance = FactorInFightingBackTraits(beatee, fightBackChance);
                }

                fightingBack = Rand.Chance(fightBackChance);

                Logger.MessageFormat(this, "{0} fighting back: {1}", beatee, fightingBack);
            }
        }

        public void TryPrisonBreak()
        {
            if (!HasValidBeatee || beatee.Downed)
                return;

            if (fightingBack)
            {
                float prisonBreakChance = basePrisonBreakChance;

                Logger.MessageFormat(this, "{0} prison break chance: {1}", beatee, prisonBreakChance);

                if (Rand.Chance(prisonBreakChance))
                    PrisonBreakUtility.StartPrisonBreak(beatee);
            }
        }

        private float FactorInFightingBackTraits(Pawn pawn, float initialValue)
        {
            TraitSet traits = pawn.story.traits;

            Logger.StartMessage(this, "{0} base fightback chance = {1}", pawn, initialValue);

            foreach (Pair<string, float> traitFactor in fightBackTraitFactors)
            {
                if (traits.allTraits.Any(trait => trait.def.defName == traitFactor.First))
                {
                    initialValue *= traitFactor.Second;

                    Logger.AddToMessage("{0} *= {1}", traitFactor.First, traitFactor.Second);
                }
            }

            Logger.AddToMessage("Final fightback chance = {0}", initialValue);
            Logger.DisplayMessage();

            return initialValue;
        }
    }
}
