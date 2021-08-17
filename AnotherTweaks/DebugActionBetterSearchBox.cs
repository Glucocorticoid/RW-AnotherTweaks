using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using HugsLib.Settings;
using Verse;
using System.Reflection;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(Dialog_DebugOptionListLister), "DoListingItems")]
    public class Dialog_DebugOptionListLister_Patch
    {
        private static FieldInfo options = AccessTools.Field(typeof(Dialog_DebugOptionListLister), "options");
        private static MethodInfo filter = AccessTools.Method(typeof(Dialog_DebugOptionListLister), "FilterAllows");

        private static bool filterAllows(Dialog_DebugOptionListLister dialog, string label)
        {
            if (filter == null) return true;
            bool result = (bool)filter.Invoke(dialog, new[] { label });
            return result;
        }

        private static List<DebugMenuOption> GetList(Dialog_DebugOptionListLister __instance)
        {
            List<DebugMenuOption> ops = options.GetValue(__instance) as List<DebugMenuOption>;
            if (ops == null) return new List<DebugMenuOption>();

            //return ops.Where(x => __instance.FilterAllows(x.label)).ToList();
            return ops.Where(x => filterAllows(__instance, x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugOptionListLister_Patch);
            var options = AccessTools.Field(typeof(Dialog_DebugOptionListLister), "options");
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugActionsMenu.DebugActionOption>));
            
            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == options)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead options my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }

    [HarmonyPatch(typeof(Dialog_DebugActionsMenu), "DoListingItems")]
    public class Dialog_DebugActionsMenu_Patch
    {
        private static FieldInfo actions = AccessTools.Field(typeof(Dialog_DebugActionsMenu), "debugActions");
        private static MethodInfo filter = AccessTools.Method(typeof(Dialog_DebugActionsMenu), "FilterAllows");

        private static bool filterAllows(Dialog_DebugActionsMenu dialog, string label)
        {
            if (filter == null) return true;
            bool result = (bool)filter.Invoke(dialog, new[] { label });
            return result;
        }

        private static List<Dialog_DebugActionsMenu.DebugActionOption> GetList(Dialog_DebugActionsMenu __instance)
        {
            List<Dialog_DebugActionsMenu.DebugActionOption> acts = actions.GetValue(__instance) as List<Dialog_DebugActionsMenu.DebugActionOption>;
            if (acts == null) return new List<Dialog_DebugActionsMenu.DebugActionOption>();

            return acts.Where(x => filterAllows(__instance, x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugActionsMenu_Patch);
            var debugActions = AccessTools.Field(typeof(Dialog_DebugActionsMenu), "debugActions");
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugActionsMenu.DebugActionOption>));
            
            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == debugActions)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead debugActions my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }

    /*[HarmonyPatch(typeof(Dialog_DebugOutputMenu), "DoListingItems")]
    public class Dialog_DebugOutputMenu_Patch
    {
        private static List<Dialog_DebugOutputMenu.DebugOutputOption> GetList(Dialog_DebugOutputMenu __instance)
        {
            return __instance.debugOutputs.Where(x => __instance.FilterAllows(x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugOutputMenu_Patch);
            var debugOutputs = AccessTools.Field(typeof(Dialog_DebugOutputMenu), "debugOutputs");
            var code = instructions.ToList();
            var innerStruct = typeof(Dialog_DebugOutputMenu).GetNestedType("DebugOutputOption");
            var optionListType = typeof(List<>).MakeGenericType(innerStruct);   
            var myList = ilGen.DeclareLocal(optionListType);
            //var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugOutputMenu.DebugOutputOption>));

            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == debugOutputs)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead debugActions my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }*/

}