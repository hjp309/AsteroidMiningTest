using System.Diagnostics;

namespace AsteroidsMining.Utils
{
    public class PerformanceMonitor
    {
        private Stopwatch gameTimer;
        private Stopwatch loopTimer;
        private List<double> loopTimes;
        private long memoryUsageStart;
        public int frameCount;

        public static List<double> AllLoopTimes = new List<double>();
        public static List<string> PerformanceLogs = new List<string>();

        public PerformanceMonitor()
        {
            gameTimer = new Stopwatch();
            loopTimer = new Stopwatch();
            loopTimes = new List<double>();
            frameCount = 0;

            // Record starting memory
            GC.Collect(); // Force garbage collection for baseline
            memoryUsageStart = GC.GetTotalMemory(false);
        }

        public void StartGame()
        {
            gameTimer.Start();
        }

        public void StartLoop()
        {
            loopTimer.Restart();
        }

        public void EndLoop()
        {
            loopTimer.Stop();
            double loopTime = loopTimer.Elapsed.TotalMilliseconds;

            loopTimes.Add(loopTime);
            AllLoopTimes.Add(loopTime);

            frameCount++;

            string logEntry = $"Frame {frameCount}: {loopTime:F2}ms";
            PerformanceLogs.Add(logEntry);
        }

        public void DisplayPerformanceStats()
        {
            if (loopTimes.Count == 0) return;

            long currentMemory = GC.GetTotalMemory(false);
            long memoryDelta = currentMemory - memoryUsageStart;

            double avgLoopTime = loopTimes.Average();
            double minLoopTime = loopTimes.Min();
            double maxLoopTime = loopTimes.Max();

            double recentAvg = loopTimes.Skip(Math.Max(0, loopTimes.Count - 60)).Average();

            Console.WriteLine("\n=== PERFORMANCE STATS ===");
            Console.WriteLine($"Game Runtime: {gameTimer.Elapsed.TotalSeconds:F1}s");
            Console.WriteLine($"Total Frames: {frameCount}");
            Console.WriteLine($"Average FPS: {frameCount / gameTimer.Elapsed.TotalSeconds:F1}");
            Console.WriteLine($"Loop Time - Avg: {avgLoopTime:F2}ms, Min: {minLoopTime:F2}ms, Max: {maxLoopTime:F2}ms");
            Console.WriteLine($"Recent Loop Avg (60 frames): {recentAvg:F2}ms");
            Console.WriteLine($"Memory Usage: {currentMemory / 1024 / 1024:F1}MB (Delta: +{memoryDelta / 1024 / 1024:F1}MB)");

            Console.WriteLine($"GC Gen 0: {GC.CollectionCount(0)}, Gen 1: {GC.CollectionCount(1)}, Gen 2: {GC.CollectionCount(2)}");
        }

        public void CheckMemoryPressure()
        {
            long memoryBefore = GC.GetTotalMemory(false);
            GC.Collect(2, GCCollectionMode.Forced);
            long memoryAfter = GC.GetTotalMemory(true);

            string pressureLog = $"Memory pressure check: {memoryBefore / 1024}KB -> {memoryAfter / 1024}KB";
            PerformanceLogs.Add(pressureLog);
        }

        public bool IsPerformanceDegrading()
        {
            if (loopTimes.Count < 120) return false;

            var early = new List<double>();
            var recent = new List<double>();

            for (int i = 0; i < 60; i++)
            {
                early.Add(loopTimes[i]);
                recent.Add(loopTimes[loopTimes.Count - 60 + i]);
            }

            double earlyAvg = early.Sum() / early.Count;
            double recentAvg = recent.Sum() / recent.Count;

            return recentAvg > earlyAvg * 1.5; // 50% slower is degrading
        }

        public double GetCurrentFPS()
        {
            if (gameTimer.Elapsed.TotalSeconds == 0) return 0;
            return frameCount / gameTimer.Elapsed.TotalSeconds;
        }

        public void LogCustomMetric(string name, double value)
        {
            string logEntry = DateTime.Now.ToString("HH:mm:ss.fff") + " - " + name + ": " + value.ToString("F2");
            PerformanceLogs.Add(logEntry);
        }

        public List<PerformanceSnapshot> GetPerformanceHistory()
        {
            var snapshots = new List<PerformanceSnapshot>();

            // Create snapshots every 30 frames
            for (int i = 0; i < loopTimes.Count; i += 30)
            {
                var snapshot = new PerformanceSnapshot
                {
                    FrameNumber = i,
                    LoopTime = loopTimes[i],
                    Timestamp = DateTime.Now.AddMilliseconds(-loopTimes.Count + i) // Approximate
                };
                snapshots.Add(snapshot);
            }

            return snapshots;
        }
    }

    public class PerformanceSnapshot
    {
        public int FrameNumber { get; set; }
        public double LoopTime { get; set; }
        public DateTime Timestamp { get; set; }

        public PerformanceSnapshot()
        {
            Timestamp = DateTime.Now;
        }
    }
}