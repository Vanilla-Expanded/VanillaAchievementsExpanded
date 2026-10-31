using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Security.Cryptography;
using UnityEngine;

namespace AchievementsExpanded
{
    public class GenericTracker : TrackerBase
    {
        public override string Key
        {
            get { return "GenericTracker"; }
            set { Key = value; }
        }

        public int count = 1;
        public string key;

        public int triggeredCount;

        public int TriggeredCount
        {
            get
            {
                return triggeredCount;
            }
            set
            {
                triggeredCount = value;
            }
        }
                       
        protected override string[] DebugText => new string[] { "Generic tracker" };

        public override Func<bool> AttachToLongTick => () => { return Trigger(); };

        public GenericTracker()
        {
        }


        public GenericTracker(GenericTracker reference) : base(reference)
        {
            Key = reference.key;
            count = reference.count;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref key, "key");
        }

        public override bool Trigger()
        {
            base.Trigger();
           
            if(triggeredCount>=count) { return true; }
            return false;

        }

        public override bool UnlockOnStartup => Trigger();




    }
}
