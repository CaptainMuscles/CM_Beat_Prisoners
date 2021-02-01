using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CM_Beat_Prisoners
{
    public class BeatPrisonersMod : Mod
    {
        private static BeatPrisonersMod _instance;
        public static BeatPrisonersMod Instance => _instance;

        public BeatPrisonersMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("CM_Beat_Prisoners");
            harmony.PatchAll();

            _instance = this;
        }
    }
}
