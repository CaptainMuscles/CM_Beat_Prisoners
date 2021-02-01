using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    public static class BeatPrisonersUtility
    {
        public static void GiveThoughtsForPrisonerBeaten(Pawn victim, Pawn perpetrator)
        {
            if (victim == null || perpetrator == null)
            {
                return;
            }

            List<ThoughtDef> otherBeatenThoughts = new List<ThoughtDef> { BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Prisoner_Beaten,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Prisoner_Beaten_Mild,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Prisoner_Beaten_Spicy };


            List<ThoughtDef> selfBeatenThoughts = new List<ThoughtDef> {  BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Self_Beaten,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Self_Beaten_Mild,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Self_Beaten_Spicy,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Self_Beaten_Kinky };

            List<ThoughtDef> gaveBeatingThoughts = new List<ThoughtDef> { BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Give_Beating_Mild,
                                                                          BeatPrisonersDefOf.CM_Beat_Prisoners_Thought_Give_Beating_Spicy };

            TryGiveThoughts(victim, selfBeatenThoughts);
            TryGiveThoughts(perpetrator, gaveBeatingThoughts);

            foreach (Pawn humanlikePawnOnMap in perpetrator.MapHeld.mapPawns.AllPawns.Where(pawn => pawn != victim && pawn != perpetrator && !pawn.NonHumanlikeOrWildMan() && pawn.needs.mood != null))
                TryGiveThoughts(humanlikePawnOnMap, otherBeatenThoughts);
        }

        private static void TryGiveThoughts(Pawn pawn, List<ThoughtDef> thoughts)
        {
            foreach (ThoughtDef thought in thoughts)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
        }
    }
}
