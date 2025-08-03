using WoWCombatLogSplit.src;

namespace WoWCombatLogSplitBenchmarks.src
{
    public class WorkerRunnerFactory
    {
        private readonly Settings Settings;
        private readonly Worker Worker;
        private readonly List<string> FilePaths = [];
        private WorkerRunnerFactory(string filePath)
        {
            Settings = new([]);
            Settings.TrySetFile(filePath);
            Settings.TrySetDir(Path.GetTempPath());
            Settings.TrySetGap(0.5);
            Worker = new(Settings);
        }
        public void Cleanup()
        {
            File.Delete(Settings.FilePathFull);
            FilePaths.ForEach(File.Delete);
            FilePaths.Clear();
        }
        public void Process()
        {
            Worker.Process((_, filePath, _) => FilePaths.Add(filePath));
        }
        public static WorkerRunnerFactory Create(int numLines)
        {
            var filePath = Path.GetTempFileName();
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8, bufferSize: 1_048_576))
            {
                new MockLog { NumLines = numLines }.Write(writer);
            }
            return new(filePath);
        }
    }
}
