using System;

namespace Utils {
    public static class Perf {
        /// <summary>
        /// Measures the time it takes for a method to be executed a specified number of times
        /// </summary>
        /// <param name="iterations">The number of times the input method should be executed</param>
        /// <param name="func">The input method</param>
        /// <returns>The execution time in seconds</returns>
        public static float Measure(int iterations, Action func) {
            PerfStopwatch stopwatch = PerfStopwatch.Start();
            for (int i = 0; i < iterations; i++) {
                func();
            }
            return stopwatch.Time;
        }
    }

    /// <summary>
    /// An object that allows measuring time elapsed from its creation.
    /// </summary>
    public class PerfStopwatch {
        public float StartTime { get; private set; }
        public float Time {
            get {
                return UnityEngine.Time.realtimeSinceStartup - StartTime;
            }
        }

        public PerfStopwatch(float startTime) {
            StartTime = startTime;
        }

        public static PerfStopwatch Start() {
            return new PerfStopwatch(UnityEngine.Time.realtimeSinceStartup);
        }
    }

}
