using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyber
{
    public class ActivityLogger
    {
        private List<string> _log = new List<string>();
        private const int MaxEntries = 50;

        public void Log(string action)
        {
            string entry = $"{DateTime.Now:HH:mm:ss} - {action}";
            _log.Add(entry);
            if (_log.Count > MaxEntries) _log.RemoveAt(0);
        }

        public string GetSummary(int count = 10)
        {
            if (_log.Count == 0) return "No activities yet.";
            var recent = _log.Skip(Math.Max(0, _log.Count - count)).Reverse().ToList();
            string output = " Recent Activities:\n";
            for (int i = 0; i < recent.Count; i++)
                output += $"  {i + 1}. {recent[i]}\n";
            return output;
        }

        public string GetStatistics()
        {
            int total = _log.Count;
            // Simple stats: count entries containing keywords
            int tasks = _log.Count(e => e.Contains("Task"));
            int reminders = _log.Count(e => e.Contains("Reminder"));
            int quizzes = _log.Count(e => e.Contains("Quiz"));
            return $" Statistics:\nTotal actions: {total}\nTasks: {tasks}\nReminders: {reminders}\nQuizzes: {quizzes}";
        }
    }
}