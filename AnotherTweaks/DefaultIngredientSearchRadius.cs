using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(typeof(Bill_Production), new[] {typeof(RecipeDef), typeof(Precept_ThingStyle) })] // constructor with RecipeDef
    public class DefaultIngredientSearchRadius
    {
        [HarmonyPostfix]
        public static void Bill_Production_Postfix(Bill_Production __instance)
        {
            __instance.ingredientSearchRadius = Settings.Get().IngredientSearchRadius;
        }
    }
}