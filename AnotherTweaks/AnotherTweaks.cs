using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using HugsLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AnotherTweaks
{
    public class HugsLoadStages : ModBase
    {
        protected override bool HarmonyAutoPatch { get; } = false;

        public override void DefsLoaded()
        {
            if (ModActive.CoreSK)
            {
                int maxRaidCount = AnotherTweaks.Settings.Get().CoreSK_MaxRaidCount;
                if (maxRaidCount > 10) CoreSK_Utils.Set_Config_MaxRaidCount(maxRaidCount);
            }
        }
    }

    [StaticConstructorOnStartup]
    public class AnotherTweaksMod : Mod
    {
        public static AnotherTweaksMod Instance;

        public AnotherTweaksMod(ModContentPack content) : base(content)
        {
            Instance = this;
            var h = new Harmony("pirateby.AnotherTweaks");
            h.PatchAll();

            LogExtended.Patch(h);

            // HandleBlockingPlants
            {
                var transpiler = new HarmonyMethod(typeof(HandleBlockingPlants), nameof(HandleBlockingPlants.HandleBlockingThingJob));
                h.Patch(AccessTools.Method(typeof(RoofUtility), nameof(RoofUtility.CanHandleBlockingThing)), transpiler: transpiler);
                h.Patch(AccessTools.Method(typeof(GenConstruct), nameof(GenConstruct.HandleBlockingThingJob)), transpiler: transpiler);
            }

            var sb = new StringBuilder();
            sb.Append($"[AnotherTweaks] Tweaks: ");
            if (ModActive.CoreSK)
            {
                var timeControls = AccessTools.Method($"SK.GlobalControlsUtility_DoTimespeedControls_Patch:Postfix");
                if (timeControls != null)
                {
                    h.Patch(timeControls, prefix: new HarmonyMethod(typeof(TPSPatch), nameof(TPSPatch.Prefix)));
                }
                sb.Append($"CoreSK ");
            }

            if (!ModActive.ShareTheLoad)
            {
                h.Patch(AccessTools.Method(typeof(ItemAvailability), nameof(ItemAvailability.ThingsAvailableAnywhere)), prefix: new HarmonyMethod(typeof(DeliverAsMuchAsPossible), nameof(DeliverAsMuchAsPossible.Prefix)));
                h.Patch(AccessTools.Method(typeof(WorkGiver_ConstructDeliverResources), "ResourceDeliverJobFor"), transpiler: new HarmonyMethod(typeof(BreakToContinue_Patch), nameof(BreakToContinue_Patch.Transpiler)));
                sb.Append($"ShareTheLoad ");
            }

            if (!ModActive.ReplaceStuff)
            {
                h.Patch(AccessTools.Method(typeof(TouchPathEndModeUtility), nameof(TouchPathEndModeUtility.IsCornerTouchAllowed)), prefix: new HarmonyMethod(typeof(CornerBuildable), nameof(CornerBuildable.Prefix)));
                h.Patch(AccessTools.Method(typeof(TouchPathEndModeUtility), nameof(TouchPathEndModeUtility.MakesOccupiedCellsAlwaysReachableDiagonally)), prefix: new HarmonyMethod(typeof(CornerMineableOkay), nameof(CornerMineableOkay.Prefix)));
                h.Patch(AccessTools.Method(typeof(GenPath), "ShouldNotEnterCell"), postfix: new HarmonyMethod(typeof(ShouldNotEnterCellPatch), nameof(ShouldNotEnterCellPatch.Postfix)));
                h.Patch(AccessTools.Method(typeof(HaulAIUtility), "TryFindSpotToPlaceHaulableCloseTo"), postfix: new HarmonyMethod(typeof(TryFindSpotToPlaceHaulableCloseToPatch), nameof(TryFindSpotToPlaceHaulableCloseToPatch.Postfix)));
                sb.Append($"ReplaceStuff ");
            }

            if (!ModActive.TDEnhancmentPack)
            {
                h.Patch(AccessTools.Method(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) }),
                    transpiler: new HarmonyMethod(typeof(SkillLearningIndicator), nameof(SkillLearningIndicator.Transpiler)));

                h.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)), postfix: new HarmonyMethod(typeof(Learn_Patch), nameof(Learn_Patch.Postfix)));

                h.Patch(AccessTools.Method(typeof(MainButtonWorker), nameof(MainButtonWorker.DoButton)), postfix: new HarmonyMethod(typeof(ResearchingIndicator), nameof(ResearchingIndicator.Postfix)));
                
                h.Patch(AccessTools.Method(typeof(ResearchManager), nameof(ResearchManager.ResearchPerformed)), postfix: new HarmonyMethod(typeof(ResearchPerformed), nameof(ResearchPerformed.Postfix)));
                sb.Append($"TDEnhancmentPack ");
            }

            if (!ModActive.MehniMiscModifications)
            {
                h.Patch(AccessTools.Method(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay)),
                    postfix: new HarmonyMethod(typeof(MehniMiscModifications),
                        nameof(MehniMiscModifications.DrawPawnGUIOverlay_Postfix)));
                h.Patch(
                    AccessTools.Method(typeof(Dialog_AssignBuildingOwner),
                        nameof(Dialog_AssignBuildingOwner.DoWindowContents)),
                    transpiler: new HarmonyMethod(typeof(MehniMiscModifications),
                        nameof(MehniMiscModifications.DoWindowContents_Transpiler)));
                sb.Append($"MehniMiscModifications ");
            }
            sb.Append($"initialized");
            Log.Message(sb.ToString());

            // DropOneWithControl
            {
                var rw = AccessTools.Method("RimWorld.ITab_Pawn_Gear:InterfaceDrop");
                var ce = AccessTools.Method("CombatExtended.ITab_Inventory:InterfaceDrop");
                var patch = new HarmonyMethod(typeof(DropOneWithControl), nameof(DropOneWithControl.InterfaceDrop)) {priority = 999999};

                if (ce != null)
                {
                    h.Patch(ce, prefix: patch);
                    Log.Message($"[AnotherTweaks] DropOneWithControl Initialized CE version");
                }
                else if (rw != null)
                {
                    h.Patch(rw, prefix: patch);
                    Log.Message($"[AnotherTweaks] DropOneWithControl Initialized");
                }
                else
                {
                    Log.Error($"[AnotherTweaks] DropOneWithControl Can't find method for patch");
                }
            }

            Log.Message($"[AnotherTweaks] Initialized");
        }

        public override void DoSettingsWindowContents(Rect rect) => Settings.Get().DoSettingsWindowContents(rect);

        public override string SettingsCategory() => "Another Tweaks";
    }
}
