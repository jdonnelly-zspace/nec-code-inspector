using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Tiny assertion helper so the tests need no packages (works offline and in CI).
    /// </summary>
    public class TestContext
    {
        private readonly List<string> _failures = new List<string>();
        private readonly List<string> _warnings = new List<string>();
        private int _checks;
        private string _currentTest = "";

        public IReadOnlyList<string> Failures => _failures;
        public IReadOnlyList<string> Warnings => _warnings;
        public int Checks => _checks;

        public void Begin(string testName)
        {
            _currentTest = testName;
        }

        public void IsTrue(bool condition, string message)
        {
            _checks++;
            if (!condition) _failures.Add($"{_currentTest}: {message}");
        }

        public void Equal<T>(T expected, T actual, string message)
        {
            _checks++;
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                _failures.Add($"{_currentTest}: {message} (expected {expected}, got {actual})");
        }

        public void Near(double expected, double actual, string message, double tolerance = 0.01)
        {
            _checks++;
            if (Math.Abs(expected - actual) > tolerance)
                _failures.Add($"{_currentTest}: {message} (expected {expected.ToString(CultureInfo.InvariantCulture)}, got {actual.ToString(CultureInfo.InvariantCulture)})");
        }

        public void Warn(string message)
        {
            _warnings.Add($"{_currentTest}: {message}");
        }

        /// <summary>
        /// Repository root: NEC_REPO_ROOT if set, otherwise the nearest parent of the current
        /// directory (then of the test binary) that contains Assets/_Project.
        /// </summary>
        public static string RepoRoot()
        {
            string fromEnv = Environment.GetEnvironmentVariable("NEC_REPO_ROOT");
            if (!string.IsNullOrEmpty(fromEnv) && Directory.Exists(Path.Combine(fromEnv, "Assets", "_Project")))
                return fromEnv;

            foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                var dir = new DirectoryInfo(start);
                while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "Assets", "_Project")))
                    dir = dir.Parent;
                if (dir != null) return dir.FullName;
            }

            throw new DirectoryNotFoundException("Could not find the repository root (Assets/_Project)");
        }
    }
}
