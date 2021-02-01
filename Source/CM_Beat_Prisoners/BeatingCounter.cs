using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CM_Beat_Prisoners
{
    public class BeatingCounter : IExposable
    {
        public Pawn beater = null;
        public int nextBeatingTick = 0;

        public bool HasValidBeater => beater != null &&
                                      !beater.Dead;

        public BeatingCounter()
        {
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref beater, "beater");
            Scribe_Values.Look(ref nextBeatingTick, "nextBeatingTick", 0);
        }
    }
}
