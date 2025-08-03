using BenchmarkDotNet.Attributes;

namespace WoWCombatLogSplitBenchmarks.src
{
    [JsonExporterAttribute.BriefCompressed]
    public class WorkerBenchmarks
    {
        private readonly List<WorkerRunnerFactory> WorkRunners = [];
        [GlobalSetup]
        public void GlobalSetup()
        {
            WorkRunners.Add(WorkerRunnerFactory.Create(000_001_000)); // 244 KB (~2ms)
            WorkRunners.Add(WorkerRunnerFactory.Create(000_010_000)); // 2.37 MB (~16ms)
            WorkRunners.Add(WorkerRunnerFactory.Create(000_100_000)); // 23.7 MB (~160ms)
            WorkRunners.Add(WorkerRunnerFactory.Create(001_000_000)); // 237 MB (~1.8s)
            //WorkRunners.Add(WorkerRunnerFactory.Create(010_000_000)); // 2.32 GB (~1m)
            //WorkRunners.Add(WorkerRunnerFactory.Create(100_000_000)); // 23.2 GB (~10m)
        }
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            WorkRunners.ForEach(workRunner => workRunner.Cleanup());
            WorkRunners.Clear();
        }
        /// <exception cref="Exception" />
        private void WorkRunnersProcess(int index)
        {
            var workRunner = WorkRunners.ElementAtOrDefault(index) ?? throw new Exception("Test was skipped.");
            workRunner.Process();
        }
        [Benchmark]
        public void WorkerTest_000_001_000()
        {
            WorkRunnersProcess(0);
        }
        [Benchmark]
        public void WorkerTest_000_010_000()
        {
            WorkRunnersProcess(1);
        }
        [Benchmark]
        public void WorkerTest_000_100_000()
        {
            WorkRunnersProcess(2);
        }
        [Benchmark]
        public void WorkerTest_001_000_000()
        {
            WorkRunnersProcess(3);
        }
        //[Benchmark]
        //public void WorkerTest_010_000_000()
        //{
        //    WorkRunnersProcess(4);
        //}
        //[Benchmark]
        //public void WorkerTest_100_000_000()
        //{
        //    WorkRunnersProcess(5);
        //}
    }
}
