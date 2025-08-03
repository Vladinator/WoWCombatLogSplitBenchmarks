namespace WoWCombatLogSplitBenchmarks.src
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            ProgramDebug.Run();
#else
            ProgramRelease.Run();
#endif
        }
    }
}
