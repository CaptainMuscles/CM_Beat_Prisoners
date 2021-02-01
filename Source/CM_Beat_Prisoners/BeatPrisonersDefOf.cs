using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace CM_Beat_Prisoners
{
    [DefOf]
    public static class BeatPrisonersDefOf
    {
        public static WorkTypeDef CM_Beat_Prisoners_WorkType_Break_Resistance;

        public static JobDef CM_Beat_Prisoners_Job_Break_Resistance;
        public static JobDef CM_Beat_Prisoners_Job_Cower;

        public static InteractionDef CM_Beat_Prisoners_Interaction_Prisoner_Threatened;
        public static InteractionDef CM_Beat_Prisoners_Interaction_Prisoner_Beating_Conclusion;

        public static ThoughtDef CM_Beat_Prisoners_Thought_Prisoner_Beaten;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Prisoner_Beaten_Mild;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Prisoner_Beaten_Spicy;

        public static ThoughtDef CM_Beat_Prisoners_Thought_Self_Beaten;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Self_Beaten_Mild;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Self_Beaten_Spicy;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Self_Beaten_Kinky;

        //public static ThoughtDef CM_Beat_Prisoners_Thought_Give_Beating;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Give_Beating_Mild;
        public static ThoughtDef CM_Beat_Prisoners_Thought_Give_Beating_Spicy;

        public static RecordDef CM_Beat_Prisoners_Record_Prisoners_Beaten;
    }
}
