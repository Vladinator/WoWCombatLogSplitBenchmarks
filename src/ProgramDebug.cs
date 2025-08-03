using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using System.Reflection;

namespace WoWCombatLogSplitBenchmarks.src
{
    public class MicroStopwatch
    {
        private readonly Stopwatch stopwatch = new();
        public void Start()
        {
            stopwatch.Restart();
        }
        public double Stop()
        {
            stopwatch.Stop();
            double nanoseconds = stopwatch.ElapsedTicks * (1_000_000_000.0 / System.Diagnostics.Stopwatch.Frequency);
            return nanoseconds;
        }

    }
    public class ProgramDebug
    {
        private static readonly string[] FormatTimeUnits = { "ns", "µs", "ms", "s", "min", "h" };
        private static string FormatTime(double nanoseconds)
        {
            double value = nanoseconds;
            int unitIndex = 0;
            while (value >= 1000 && unitIndex < FormatTimeUnits.Length - 1)
            {
                value /= (unitIndex == 0) ? 1000.0
                       : (unitIndex == 1) ? 1000.0
                       : (unitIndex == 2) ? 1000.0
                       : (unitIndex == 3) ? 60.0
                       : 60.0;
                unitIndex++;
            }
            return $"{value:0.###} {FormatTimeUnits[unitIndex]}";
        }
        private static Dictionary<string, Action> GetTests()
        {
            Dictionary<string, Action> tests = [];
            var instance = new WorkerBenchmarks();
            var allMethods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var setupMethod = allMethods.Where(o => o.GetCustomAttribute<GlobalSetupAttribute>() != null).FirstOrDefault();
            var cleanupMethod = allMethods.Where(o => o.GetCustomAttribute<GlobalCleanupAttribute>() != null).FirstOrDefault();
            var testMethods = allMethods.Where(o => o.GetCustomAttribute<BenchmarkAttribute>() != null);
            if (setupMethod != null)
            {
                tests.Add(setupMethod.Name, () => setupMethod.Invoke(instance, null));
            }
            foreach (var method in allMethods)
            {
                if (setupMethod == method || cleanupMethod == method)
                {
                    continue;
                }
                tests.Add(method.Name, () => method.Invoke(instance, null));
            }
            if (cleanupMethod != null)
            {
                tests.Add(cleanupMethod.Name, () => cleanupMethod.Invoke(instance, null));
            }
            return tests;
        }
        public static void Run()
        {
            var tests = GetTests();
            var testWarmups = 0;
            var testRuns = 1;
            foreach (var test in tests)
            {
                if (test.Key.StartsWith("Global"))
                {
                    test.Value();
                    continue;
                }
                double nanoseconds = -1;
                double nanosecondsMin = -1;
                double nanosecondsMax = -1;
                double numRuns = 0;
                double totalTime = 0;
                for (var i = 1; i <= testWarmups + testRuns; i++)
                {
                    if (i <= testWarmups)
                    {
                        test.Value();
                        continue;
                    }
                    MicroStopwatch stopwatch = new();
                    stopwatch.Start();
                    test.Value();
                    nanoseconds = stopwatch.Stop();
                    numRuns++;
                    totalTime += nanoseconds;
                    nanosecondsMin = nanosecondsMin == -1 ? nanoseconds : Math.Min(nanoseconds, nanosecondsMax);
                    nanosecondsMax = nanosecondsMax == -1 ? nanoseconds : Math.Max(nanoseconds, nanosecondsMax);
                }
                nanoseconds = totalTime / numRuns;
                Console.WriteLine($"{test.Key} | {numRuns:F0} runs | {FormatTime(nanoseconds)} Avg | {FormatTime(nanosecondsMin)} Min | {FormatTime(nanosecondsMax)} Max");
            }
        }
    }
}
