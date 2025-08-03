using BenchmarkDotNet.Running;

namespace WoWCombatLogSplitBenchmarks.src
{
    public class ProgramRelease
    {
        public static void Run()
        {
            BenchmarkRunner.Run<WorkerBenchmarks>();
        }
    }
}
