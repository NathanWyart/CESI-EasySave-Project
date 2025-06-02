using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace EasySaveV3._0.EasySave.ThreadManagement
{
    public static class ThreadSignals
    {
        public static ConcurrentDictionary<string, ManualResetEvent> Pausers { get; } = new();
        public static ConcurrentDictionary<string, CancellationTokenSource> Cancellers { get; } = new();

        public static void InitSignals(string workName)
        {
            Pausers[workName] = new ManualResetEvent(true);
            Cancellers[workName] = new CancellationTokenSource();
        }

        public static void RemoveSignals(string workName)
        {
            if (Pausers.TryRemove(workName, out var p)) p.Dispose();
            if (Cancellers.TryRemove(workName, out var c)) c.Dispose();
        }
    }
}

