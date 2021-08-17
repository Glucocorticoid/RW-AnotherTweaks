using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System;

namespace AnotherTweaks
{
    public class DropOneWithControl
    {
        private static Pawn GetSelectedPawn()
        {
            var thing = Find.Selector.SingleSelectedThing;
            if (thing is Pawn p)
                return p;
            if (thing is Corpse c)
                return c.InnerPawn;
            throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + thing);
        }

        public static bool InterfaceDrop(ITab_Pawn_Gear __instance, Thing t)
        {
            if (!Event.current.control || t.stackCount < 2)
                return true;

            Pawn selPawnForGear = GetSelectedPawn(); // Traverse.Create(__instance).Property("SelPawnForGear", null).GetValue<Pawn>();

            ThingWithComps thingWithComps = t as ThingWithComps;
            if (t is Apparel apparel && selPawnForGear.apparel != null && selPawnForGear.apparel.WornApparel.Contains(apparel))
            {
                // RemoveApparel
            }
            else if (thingWithComps != null && selPawnForGear.equipment != null && selPawnForGear.equipment.AllEquipmentListForReading.Contains(thingWithComps))
            {
                // DropEquipment
            }
            else if (!t.def.destroyOnDrop)
            {
                selPawnForGear.inventory.innerContainer.TryDrop(t, selPawnForGear.Position, selPawnForGear.Map, ThingPlaceMode.Near, 1, out _);
            }

            return false;
        }
    }
}