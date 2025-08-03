using WoWCombatLogSplit.src;

namespace WoWCombatLogSplitBenchmarks.src
{
    public class MockLog()
    {
        public DateTime StartDate = DateTime.Parse("2020-01-01T13:00:00");
        public int NumLines = 10;
        public double TimestampInterval = 100;
        public int FakeEventData = 220;
        public int NumGaps = 4;
        public double Gap = 0.5;
        private string CreateFakeData(Random random, string? append = null)
        {
            List<char> chars = [];
            while (chars.Count < FakeEventData)
            {
                var chr = (char)random.Next('A', 'Z' + 1);
                chars.Add(chr);
            }
            var data = new string([.. chars]);
            if (append == null)
            {
                return data;
            }
            return $"{data}{append}";
        }
        public void Write(StreamWriter writer)
        {
            var dateTime = StartDate;
            var random = new Random();
            var gapInterval = NumLines / NumGaps;
            var gapAfter = gapInterval;
            var lineCount = 0;
            while (lineCount++ < NumLines)
            {
                var timestamp = dateTime.ToString(Constants.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                var fakeData = CreateFakeData(random, "EOL");
                var line = $"{timestamp}  {fakeData}";
                writer.WriteLine(line);
                var addMilliseconds = TimestampInterval;
                if (--gapAfter <= 0)
                {
                    gapAfter = gapInterval;
                    addMilliseconds = Gap * 3600000;
                }
                dateTime = dateTime.AddMilliseconds(addMilliseconds);
            }
        }
    }
}
