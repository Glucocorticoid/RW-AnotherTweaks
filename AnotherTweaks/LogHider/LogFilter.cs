using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(Log))]
    public static class Log_Patch
    {
        [HarmonyPatch(nameof(Log.Error), new[] { typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Error(string text, bool ignoreStopLoggingLimit)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.ErrorContainFilters.ContainFilter(text) && !cfg.ErrorHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }
        [HarmonyPatch(nameof(Log.Warning), new[] { typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Warning(string text, bool ignoreStopLoggingLimit)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.WarningContainFilters.ContainFilter(text) && !cfg.WarningHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }
        [HarmonyPatch(nameof(Log.Message), new[] { typeof(string), typeof(bool) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Message(string text, bool ignoreStopLoggingLimit)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.MessageContainFilters.ContainFilter(text) && !cfg.MessageHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }

        public static bool ContainFilter(this string[] filter, string msg)
        {
            foreach (var s in filter)
            {
                if (msg.Contains(s)) return true;
            }
            return false;
        }

        public static bool ContainFilter(this string filterWithDelimiter, string msg)
        {
            var filter = filterWithDelimiter.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in filter)
            {
                if (msg.Contains(s)) return true;
            }
            return false;
        }
    }

    public class LogFilter : IExposable
    {
        public bool enabled;
        public string errorContain;
        public string warningContain;
        public string messageContain;
        private List<LogMessageHided> _logsHided = new List<LogMessageHided>();
        
        public int[] ErrorHashFilters { get; private set; }
        public int[] WarningHashFilters { get; private set; }
        public int[] MessageHashFilters { get; private set; }
        public string[] ErrorContainFilters { get; private set; }
        public string[] WarningContainFilters { get; private set; }
        public string[] MessageContainFilters { get; private set; }

        public IEnumerable<LogMessageHided> LogsHided => _logsHided;

        public void SetLogsHided(List<LogMessageHided> logsHided)
        {
            _logsHided = logsHided;
            UpdateHashes();
        }

        public void UpdateHashes()
        {
            ErrorHashFilters = _logsHided.Where(l => l.type == LogMessageType.Error).Select(l => l.text.GetHashCode()).ToArray();
            WarningHashFilters = _logsHided.Where(l => l.type == LogMessageType.Warning).Select(l => l.text.GetHashCode()).ToArray();
            MessageHashFilters = _logsHided.Where(l => l.type == LogMessageType.Message).Select(l => l.text.GetHashCode()).ToArray();
            
            if (!String.IsNullOrWhiteSpace(errorContain))
                ErrorContainFilters = errorContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else ErrorContainFilters = new string[]{};

            if (!String.IsNullOrWhiteSpace(warningContain))
                WarningContainFilters = warningContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else WarningContainFilters = new string[]{};
            
            if (!String.IsNullOrWhiteSpace(messageContain))
                MessageContainFilters = messageContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else MessageContainFilters = new string[]{};
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled");
            Scribe_Values.Look(ref errorContain, "errorContain");
            Scribe_Values.Look(ref warningContain, "warningContain");
            Scribe_Values.Look(ref messageContain, "messageContain");
            Scribe_Collections.Look(ref _logsHided, "logsHided", LookMode.Deep);
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (_logsHided == null) _logsHided = new List<LogMessageHided>();
                UpdateHashes();
            }
        }
    }
}