using Verse;
using System;

namespace AnotherTweaks
{
    public class ModActive
    {
        private static bool? _tdEnhancmentPack, _mehniMiscModifications, _coreSk, _shareTheLoad, _replaceStuff;

        public static bool TDEnhancmentPack
        {
            get
            {
                if (_tdEnhancmentPack == null)
                    _tdEnhancmentPack = LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Equals("Uuugggg.TDPack", StringComparison.CurrentCultureIgnoreCase) || x.PackageId.Equals("DEBUuugggg.TDPack", StringComparison.CurrentCultureIgnoreCase));
                return (bool) _tdEnhancmentPack;
            }
        }

        public static bool MehniMiscModifications
        {
            get
            {
                if (_mehniMiscModifications == null)
                    _mehniMiscModifications = LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Equals("Mehni.Misc.Modifications", StringComparison.CurrentCultureIgnoreCase));
                return (bool) _mehniMiscModifications;
            }
        }

        public static bool CoreSK
        {
            get
            {
                if (_coreSk == null)
                    _coreSk = LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Equals("skyarkhangel.HSK", StringComparison.CurrentCultureIgnoreCase));
                return (bool) _coreSk;
            }
        }

        public static bool ShareTheLoad
        {
            get
            {
                if (_shareTheLoad == null)
                    _shareTheLoad = LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Equals("Uuugggg.ShareTheLoad", StringComparison.CurrentCultureIgnoreCase) || x.PackageId.Equals("DEBUuugggg.ShareTheLoad", StringComparison.CurrentCultureIgnoreCase));
                return (bool) _shareTheLoad;
            }
        }

        public static bool ReplaceStuff
        {
            get
            {
                if (_replaceStuff == null)
                    _replaceStuff = LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Equals("Uuugggg.ReplaceStuff", StringComparison.CurrentCultureIgnoreCase) || x.PackageId.Equals("DEBUuugggg.ReplaceStuff", StringComparison.CurrentCultureIgnoreCase));
                return (bool) _replaceStuff;
            }
        }
    }
}