using System.Collections.Concurrent;

namespace EasySaveV3._0.EasySave.ThreadManagement
{
    public static class BusinessSignals
    {
        public static ConcurrentDictionary<string, ManualResetEvent> Pausers { get; } = new();

        public static bool AnyRunning()
        {
            return Pausers.Count > 0;
        }

        public static void InitSignal(string workName)
        {
            Pausers[workName] = new ManualResetEvent(true);
        }

        public static void PauseAll()
        {
            foreach (var kvp in Pausers)
                kvp.Value.Reset();
        }

        public static void ResumeAll()
        {
            foreach (var kvp in Pausers)
                kvp.Value.Set();
        }

        public static void RemoveSignal(string workName)
        {
            if (Pausers.TryRemove(workName, out var signal))
                signal.Dispose();
        }
    }
}