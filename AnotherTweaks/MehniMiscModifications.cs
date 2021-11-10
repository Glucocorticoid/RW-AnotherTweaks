using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AnotherTweaks
{
    public class MehniMiscModifications
    {
        #region BetterHostileReadouts
        /// <summary>
        /// Show label names on hostile animals. Credits: mehni. 4misc mods
        /// </summary>
        /// <param name="___pawn"></param>
        public static void DrawPawnGUIOverlay_Postfix(Pawn ___pawn)
        {
            // First two checks are just to prevent duplicates
            if (Settings.Get().BetterHostileReadouts && !___pawn.RaceProps.Humanlike && ___pawn.Faction != Faction.OfPlayer && ___pawn.HostileTo(Faction.OfPlayer))
                GenMapUI.DrawPawnLabel(___pawn, GenMapUI.LabelDrawPosFor(___pawn, -0.6f), font: GameFont.Tiny);
        }
        #endregion BetterHostileReadouts

        #region showLovers
        public static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator gen)
        {
            var label = gen.DefineLabel();
            MethodInfo widgetsLabel = AccessTools.Method(typeof(Widgets), nameof(Widgets.Label), new Type[] { typeof(Rect), typeof(string) });
            MethodInfo getPawnName = AccessTools.Property(typeof(Entity), nameof(Entity.LabelCap)).GetGetMethod();
            MethodInfo endScrollView = AccessTools.Method(typeof(Widgets), nameof(Widgets.EndScrollView));

            List<CodeInstruction> instructionList = codeInstructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                // We want to inject the same code in two spots.

                // insert after `void Widgets.DrawLabel` - stack is neutral, so we can branch away
                if (i > 1 && instructionList[i - 1].Calls(getPawnName) && instructionList[i].Calls(widgetsLabel)) // last inst won't be a call to getPawnName
                {
                    yield return instructionList[i];
                    foreach (var inst in InsertDrawHeart(label, new List<CodeInstruction> { instructionList[i - 2] }))
                        yield return inst;
                }
                // insert after `bool accepted = acceptanceReport.Accepted`, stack is neutral here.
                else if (i < instructionList.Count - 4 && instructionList[i + 3].Calls(getPawnName) && !instructionList[i + 4].Calls(widgetsLabel))
                {
                    yield return instructionList[i];

                    foreach (var inst in InsertDrawHeart(label, new List<CodeInstruction> { instructionList[i + 1], instructionList[i + 2] }))
                        yield return inst;
                }
                else
                {
                    if (instructionList[i].Calls(endScrollView))
                        yield return instructionList[i].WithLabels(label);
                    else
                        yield return instructionList[i];
                }
            }
        }

        private static float resizeHeart = 0.50f;
        private static float offsetPosition = 0.62f;
        private static MethodInfo makeReeect = AccessTools.Method(typeof(MehniMiscModifications), nameof(MehniMiscModifications.ShowHeart));
        private static MethodInfo getViewRectWidth = AccessTools.Property(typeof(Rect), nameof(Rect.width)).GetGetMethod();

        private static IEnumerable<CodeInstruction> InsertDrawHeart(Label label, List<CodeInstruction> getPawn)
        {
            yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
            yield return new CodeInstruction(OpCodes.Call, getViewRectWidth);
            yield return new CodeInstruction(OpCodes.Ldc_R4, offsetPosition);
            yield return new CodeInstruction(OpCodes.Mul); //viewrect.width * 0.55
            yield return new CodeInstruction(OpCodes.Ldloc_2); //y
            foreach (var inst in getPawn)
                yield return inst;
            yield return new CodeInstruction(OpCodes.Call, makeReeect);
            yield return new CodeInstruction(OpCodes.Brtrue, label); // leave.s   
        }

        private static bool ShowHeart(float x, float y, Pawn pawn)
        {
            Texture2D iconFor;
            if (pawn == null || !pawn.IsColonist)
                return false;

            DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(pawn, false);
            if (directPawnRelation == null || directPawnRelation.otherPawn == null)
                iconFor = null;

            else if (!directPawnRelation.otherPawn.IsColonist || directPawnRelation.otherPawn.IsWorldPawn() || !directPawnRelation.otherPawn.relations.everSeenByPlayer)
                iconFor = null;

            else if (pawn.ownership?.OwnedBed != null && pawn.ownership?.OwnedBed == directPawnRelation.otherPawn.ownership?.OwnedBed)
                iconFor = Textures.BondIcon;

            else
                iconFor = Textures.BondBrokenIcon;

            if (iconFor != null)
            {
                Vector2 iconSize = new Vector2(iconFor.width, iconFor.height) * resizeHeart;
                Rect drawRect = new Rect(x, y, iconSize.x, iconSize.y);
                TooltipHandler.TipRegion(drawRect, directPawnRelation.otherPawn.LabelCap);

                if (iconFor == Textures.BondBrokenIcon) // if its a broken heart - allow them to click on the broken heart to assign the missing partner to the bed
                {
                    if (Widgets.ButtonImage(drawRect, iconFor, Color.white, Color.red, true))
                    {
                        if (pawn.ownership?.OwnedBed?.SleepingSlotsCount >= 2)
                        {
                            pawn.ownership.OwnedBed.GetComp<CompAssignableToPawn>().TryAssignPawn(directPawnRelation.otherPawn);
                            return true;
                        }
                    }
                }
                else
                {
                    GUI.DrawTexture(drawRect, iconFor);
                }

            }
            return false;
        }
        #endregion showLovers
    }
}