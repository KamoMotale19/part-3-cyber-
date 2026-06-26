using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cyber
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TaskManager
    {
        private string _connectionString;
        private List<TaskItem> _localTasks = new List<TaskItem>();
        private bool _useDatabase = true;

        public TaskManager(string connString)
        {
            _connectionString = connString;
            try
            {
                // Test connection
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                // Ensure table exists
                string createTable = @"CREATE TABLE IF NOT EXISTS Tasks (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    ReminderDate DATETIME NULL,
                    IsCompleted BOOLEAN DEFAULT FALSE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
                using var cmd = new MySqlCommand(createTable, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                _useDatabase = false; // Fallback to local storage
            }
        }

        public bool AddTask(string title, string description, DateTime? reminder)
        {
            if (_useDatabase)
            {
                try
                {
                    using var conn = new MySqlConnection(_connectionString);
                    conn.Open();
                    string query = "INSERT INTO Tasks (Title, Description, ReminderDate) VALUES (@t, @d, @r)";
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@d", description ?? "");
                    cmd.Parameters.AddWithValue("@r", reminder ?? (object)DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
            else
            {
                _localTasks.Add(new TaskItem
                {
                    Id = _localTasks.Count + 1,
                    Title = title,
                    Description = description ?? "",
                    ReminderDate = reminder,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                });
                return true;
            }
        }

        public List<TaskItem> GetTasks(bool includeCompleted = false)
        {
            if (_useDatabase)
            {
                var tasks = new List<TaskItem>();
                try
                {
                    using var conn = new MySqlConnection(_connectionString);
                    conn.Open();
                    string query = includeCompleted ? "SELECT * FROM Tasks ORDER BY CreatedAt DESC"
                                                   : "SELECT * FROM Tasks WHERE IsCompleted = FALSE ORDER BY CreatedAt DESC";
                    using var cmd = new MySqlCommand(query, conn);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull("ReminderDate") ? null : reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
                catch { /* fallback to local if DB fails */ }
                if (tasks.Count > 0) return tasks;
                // If DB empty or failed, return local
            }
            // Return local tasks (if DB not used or empty)
            return _localTasks.Where(t => includeCompleted || !t.IsCompleted).ToList();
        }

        public bool MarkComplete(int id)
        {
            if (_useDatabase)
            {
                try
                {
                    using var conn = new MySqlConnection(_connectionString);
                    conn.Open();
                    string query = "UPDATE Tasks SET IsCompleted = TRUE WHERE Id = @id";
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
            else
            {
                var task = _localTasks.FirstOrDefault(t => t.Id == id);
                if (task != null) { task.IsCompleted = true; return true; }
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            if (_useDatabase)
            {
                try
                {
                    using var conn = new MySqlConnection(_connectionString);
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @id";
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
            else
            {
                var task = _localTasks.FirstOrDefault(t => t.Id == id);
                if (task != null) { _localTasks.Remove(task); return true; }
                return false;
            }
        }

        public bool SetReminder(int id, DateTime reminderDate)
        {
            if (_useDatabase)
            {
                try
                {
                    using var conn = new MySqlConnection(_connectionString);
                    conn.Open();
                    string query = "UPDATE Tasks SET ReminderDate = @r WHERE Id = @id";
                    using var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@r", reminderDate);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
            else
            {
                var task = _localTasks.FirstOrDefault(t => t.Id == id);
                if (task != null) { task.ReminderDate = reminderDate; return true; }
                return false;
            }
        }
    }
}