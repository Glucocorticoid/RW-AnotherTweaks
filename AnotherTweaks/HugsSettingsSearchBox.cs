using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using HugsLib.Settings;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    /*[HarmonyPatch(typeof(Dialog_ModSettings), nameof(Dialog_ModSettings.DoWindowContents))]
    public class HugsSettingsSearchBox
    {
        private const float SearchBoxWidth = 300f;
        private const string SearchCtrlName = "##SearchStrCtrl";

        private static GUIStyle _defaultStyle = new GUIStyle(Text.CurTextFieldStyle);
        private static string _searchStr = string.Empty;
        private static bool _searchFocused;
        private static List<Dialog_ModSettings.ModEntry> _cachedFiltered;

        private static List<Dialog_ModSettings.ModEntry> GetList(Dialog_ModSettings __instance)
        {
            if (string.IsNullOrEmpty(_searchStr))
                return __instance.listedMods;
            if (_cachedFiltered == null)
            {
                var searchKey = _searchStr.ToLower();
                _cachedFiltered = __instance.listedMods.Where(x => x.ModName.ToLower().Contains(searchKey)).ToList();
            }
            return _cachedFiltered;
        }

        private static void DoSearchBlock(Rect inRect) // RSA search box
        {
            Rect area = new Rect(inRect.width / 2 - SearchBoxWidth / 2, inRect.height - 24f, SearchBoxWidth, 24f);
            float num = 12f * Math.Min(1f, area.height / 29f);
            Rect butRect = new Rect(area.xMax - 4f - num, area.y + (area.height - num) / 2f, num, num);
            bool resetSearch = Widgets.ButtonImage(butRect, Widgets.CheckboxOffTex, true);
            string text = (_searchStr != string.Empty || _searchFocused) ? _searchStr : "Search";
            bool isEscapePressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            bool isClickedOutside = !Mouse.IsOver(area) && Event.current.type == EventType.MouseDown;
            if (!_searchFocused)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
            }
            GUI.SetNextControlName(SearchCtrlName);
            string value = GUI.TextField(area, text, _defaultStyle);
            GUI.color = Color.white;
            if (_searchFocused)
            {
                if (!_searchStr.Equals(value))
                {
                    _cachedFiltered = null;
                }
                _searchStr = value;
            }
            if ((GUI.GetNameOfFocusedControl() == SearchCtrlName || _searchFocused) && (isEscapePressed || isClickedOutside))
            {
                GUIUtility.keyboardControl = 0;
                _searchFocused = false;
            }
            else
            {
                if (GUI.GetNameOfFocusedControl() == SearchCtrlName && !_searchFocused)
                {
                    _searchFocused = true;
                }
            }
            if (resetSearch)
            {
                _cachedFiltered = null;
                _searchStr = string.Empty;
            }
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(HugsSettingsSearchBox);
            var listedMods = AccessTools.Field(typeof(Dialog_ModSettings), "listedMods");
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_ModSettings.ModEntry>));
            bool firstEntryPassed = false;

            // prefix
            yield return new CodeInstruction(OpCodes.Ldarg_1); // inRect
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(DoSearchBlock)));

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == listedMods)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);

                    if (!firstEntryPassed)
                    {
                        // get filtered listedMods in to my local var
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
                        yield return new CodeInstruction(OpCodes.Stloc, myList);
                        firstEntryPassed = true;
                    }

                    // use instead listedMods my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }*/
}