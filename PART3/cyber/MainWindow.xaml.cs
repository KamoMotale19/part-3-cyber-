using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cyber
{


    public partial class MainWindow : Window
    {
        private Chatbot bot = new Chatbot();
        private SpeechSynthesizer speechSynthesizer;
        private bool voiceEnabled = true;
        private int messageCount = 0;
        private string currentUserName = "You";
        private TaskManager _taskManager;
        private QuizManager _quizManager;
        private ActivityLogger _logger;
        private bool _waitingForQuizAnswer = false;



        public MainWindow()
        {
            InitializeComponent();
            string connString = "server=localhost;database=ChatbotDB;uid=root;pwd=yourpassword;";
            _taskManager = new TaskManager(connString);
            _quizManager = new QuizManager();
            _logger = new ActivityLogger();
            _logger.Log("Chatbot started");
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.Rate = -1;
            speechSynthesizer.Volume = 100;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayBotMessage("Welcome to SECUREX!");
            DisplayBotMessage("What is your name?");
            AppendText("=== SECUREX CYBER DEFENSE SYSTEM ===\n\n", Brushes.Cyan);
            txtUserInput.Focus();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void BtnVoice_Click(object sender, RoutedEventArgs e)
        {
            voiceEnabled = !voiceEnabled;

            if (voiceEnabled)
            {
                btnVoice.Content = "Voice ON";
                btnVoice.Background = new SolidColorBrush(Color.FromRgb(0, 150, 100));
            }
            else
            {
                btnVoice.Content = "Voice OFF";
                btnVoice.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            }
        }

        private void SendMessage()
        {
            string userInput = txtUserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please type a message!", "Empty Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DisplayUserMessage(userInput, currentUserName);

            // --- Special commands ---
            string lower = userInput.ToLower();

            // 1. Activity Log
            if (lower.Contains("activity log") || lower.Contains("what have you done") || lower.Contains("show log"))
            {
                DisplayBotMessage(_logger.GetSummary(10));
                goto End;
            }

            // 2. Statistics
            if (lower.Contains("stats") || lower.Contains("statistics"))
            {
                DisplayBotMessage(_logger.GetStatistics());
                goto End;
            }

            // 3. Start Quiz
            if (lower.Contains("start quiz") || lower.Contains("play quiz") || lower.Contains("take quiz"))
            {
                if (_quizManager.IsActive)
                {
                    DisplayBotMessage("A quiz is already in progress. Answer the current question (A/B/C/D) or type 'quit quiz'.");
                    goto End;
                }
                if (_quizManager.StartQuiz())
                {
                    _logger.Log("Started quiz");
                    DisplayBotMessage("🎮 Cybersecurity Quiz Started!\n" + _quizManager.GetCurrentQuestion());
                    _waitingForQuizAnswer = true;
                }
                goto End;
            }

            // 4. Handle quiz answer if quiz is active
            if (_quizManager.IsActive)
            {
                if (lower == "quit quiz")
                {
                    _quizManager = new QuizManager(); // reset
                    DisplayBotMessage("Quiz ended. You can start a new one anytime.");
                    _waitingForQuizAnswer = false;
                    goto End;
                }
                string feedback = _quizManager.SubmitAnswer(userInput);
                DisplayBotMessage(feedback);
                if (!_quizManager.IsActive)
                {
                    _logger.Log($"Completed quiz with score {_quizManager.GetScore()}/{_quizManager.GetTotal()}");
                    _waitingForQuizAnswer = false;
                }
                goto End;
            }

            // 5. Task Commands
            if (lower.Contains("add task") || lower.Contains("new task"))
            {
                // Extract title after "add task" or "new task"
                string title = userInput;
                foreach (string phrase in new[] { "add task", "new task" })
                {
                    if (lower.Contains(phrase))
                    {
                        int idx = lower.IndexOf(phrase) + phrase.Length;
                        title = userInput.Substring(idx).Trim();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(title))
                {
                    DisplayBotMessage("Please specify a task title. Example: 'Add task: Enable 2FA'");
                }
                else
                {
                    if (_taskManager.AddTask(title, "", null))
                    {
                        DisplayBotMessage($" Task added: '{title}'. You can set a reminder later.");
                        _logger.Log($"Added task: {title}");
                    }
                    else
                    {
                        DisplayBotMessage(" Could not add task. Please check database connection.");
                    }
                }
                goto End;
            }

            if (lower.Contains("show tasks") || lower.Contains("list tasks") || lower.Contains("view tasks"))
            {
                var tasks = _taskManager.GetTasks(false);
                if (tasks.Count == 0)
                {
                    DisplayBotMessage("No pending tasks. Good job!");
                }
                else
                {
                    string msg = " Your pending tasks:\n";
                    foreach (var t in tasks)
                    {
                        msg += $"  #{t.Id} - {t.Title}";
                        if (t.ReminderDate.HasValue) msg += $" (Reminder: {t.ReminderDate:g})";
                        msg += "\n";
                    }
                    DisplayBotMessage(msg);
                }
                goto End;
            }

            if (lower.Contains("complete task") || lower.Contains("mark task complete"))
            {
                // Try to extract task ID (first number)
                int id = 0;
                foreach (string word in userInput.Split(' '))
                    if (int.TryParse(word, out id)) break;
                if (id > 0)
                {
                    if (_taskManager.MarkComplete(id))
                    {
                        DisplayBotMessage($" Task #{id} marked as completed.");
                        _logger.Log($"Completed task #{id}");
                    }
                    else
                    {
                        DisplayBotMessage($" Task #{id} not found or could not be completed.");
                    }
                }
                else
                {
                    DisplayBotMessage("Please specify the task ID. Example: 'Complete task 3'");
                }
                goto End;
            }

            if (lower.Contains("delete task"))
            {
                int id = 0;
                foreach (string word in userInput.Split(' '))
                    if (int.TryParse(word, out id)) break;
                if (id > 0)
                {
                    if (_taskManager.DeleteTask(id))
                    {
                        DisplayBotMessage($" Task #{id} deleted.");
                        _logger.Log($"Deleted task #{id}");
                    }
                    else
                    {
                        DisplayBotMessage($" Task #{id} not found.");
                    }
                }
                else
                {
                    DisplayBotMessage("Please specify the task ID. Example: 'Delete task 3'");
                }
                goto End;
            }

            if (lower.Contains("set reminder") || lower.Contains("remind me"))
            {
                // Parse: "set reminder for task 3 in 5 days" – try to find task ID and days
                int id = 0, days = 0;
                foreach (string word in userInput.Split(' '))
                {
                    if (int.TryParse(word, out int num))
                    {
                        if (id == 0) id = num;
                        else days = num;
                    }
                }
                if (id > 0 && days > 0)
                {
                    DateTime reminderDate = DateTime.Now.AddDays(days);
                    if (_taskManager.SetReminder(id, reminderDate))
                    {
                        DisplayBotMessage($" Reminder set for task #{id} on {reminderDate:g}.");
                        _logger.Log($"Set reminder for task #{id}");
                    }
                    else
                    {
                        DisplayBotMessage($" Could not set reminder for task #{id}.");
                    }
                }
                else
                {
                    DisplayBotMessage("Please specify: 'Set reminder for task [id] in [days] days'.");
                }
                goto End;
            }

           
            string botResponse = bot.GetResponse(userInput);
            DisplayBotMessage(botResponse);
            if (voiceEnabled) SpeakMessage(botResponse);

            UpdateSentimentDisplay(userInput);
            UpdateMemoryDisplay(userInput);

            
            _logger.Log($"User asked: {userInput.Substring(0, Math.Min(userInput.Length, 30))}...");

        End:
            messageCount++;
            lblMessageCount.Text = "Messages: " + messageCount;
            txtUserInput.Clear();
            txtUserInput.Focus();
        }

        private void AppendText(string text, Brush color)
        {
            TextRange range = new TextRange(rtbChatDisplay.Document.ContentEnd, rtbChatDisplay.Document.ContentEnd);
            range.Text = text;
            range.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            rtbChatDisplay.ScrollToEnd();
        }

        private void DisplayUserMessage(string message, string userName)
        {
            AppendText(userName + ": ", Brushes.LightSkyBlue);
            AppendText(message + "\n\n", Brushes.White);
        }

        private void DisplayBotMessage(string message)
        {
            message = message.Replace("\\n", "\n");
            AppendText("SECUREX: ", Brushes.LightGreen);
            AppendText(message + "\n\n", Brushes.LightGray);

            // Speak the message if voice is enabled
            if (voiceEnabled)
                SpeakMessage(message);
        }

        private void SpeakMessage(string message)
        {
            try
            {
                speechSynthesizer.SpeakAsyncCancelAll();
                string cleanMessage = message.Replace("\n", " ");
                cleanMessage = System.Text.RegularExpressions.Regex.Replace(cleanMessage, @"[^\w\s\-,.]", "");

                if (!string.IsNullOrWhiteSpace(cleanMessage))
                    speechSynthesizer.SpeakAsync(cleanMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Voice error: " + ex.Message);
            }
        }

        private void UpdateSentimentDisplay(string input)
        {
            string sentiment = "Sentiment: Neutral";
            string inputLower = input.ToLower();

            if (inputLower.Contains("worried") || inputLower.Contains("scared") || inputLower.Contains("afraid"))
                sentiment = "Sentiment: Worried";
            else if (inputLower.Contains("frustrated") || inputLower.Contains("angry") || inputLower.Contains("annoyed"))
                sentiment = "Sentiment: Frustrated";
            else if (inputLower.Contains("curious") || inputLower.Contains("interested") || inputLower.Contains("want to know"))
                sentiment = "Sentiment: Curious";
            else if (inputLower.Contains("happy") || inputLower.Contains("great") || inputLower.Contains("excellent"))
                sentiment = "Sentiment: Happy";
            else if (inputLower.Contains("sad") || inputLower.Contains("bad") || inputLower.Contains("upset"))
                sentiment = "Sentiment: Sad";

            lblSentiment.Text = sentiment;
        }

        private void BtnTasks_Click(object sender, RoutedEventArgs e)
        {
            txtUserInput.Text = "show tasks";
            SendMessage();
        }
        private void BtnQuiz_Click(object sender, RoutedEventArgs e)
        {
            txtUserInput.Text = "start quiz";
            SendMessage();
        }
        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            txtUserInput.Text = "activity log";
            SendMessage();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            rtbChatDisplay.Document.Blocks.Clear();
            DisplayBotMessage("Chat cleared. How can I help you?");
        }

        private void UpdateTaskCount()
        {
            var tasks = _taskManager.GetTasks(false);
            lblTaskCount.Text = $"Tasks: {tasks.Count} pending";
        }

        private void UpdateQuizProgress()
        {
            if (_quizManager.IsActive)
                lblQuizProgress.Text = $"Quiz: {_quizManager.GetProgress():P0}";
            else
                lblQuizProgress.Text = "Quiz: Not started";
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop background music if playing (if you added MediaPlayer)
            // _bgMusic?.Stop();

            // Dispose speech synthesizer
            speechSynthesizer?.Dispose();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            txtUserInput.Text = "help";
            SendMessage();
        }

        private void UpdateMemoryDisplay(string input)
        {
            string inputLower = input.ToLower();

            if (inputLower.Contains("my name is"))
            {
                try
                {
                    string name = inputLower.Split("my name is")[1].Trim();
                    name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Split(' ')[0]);
                    lblUserName.Text = "User: " + name;
                    currentUserName = name;
                }
                catch { }
            }

            if (inputLower.Contains("i am interested in") || inputLower.Contains("i like"))
            {
                try
                {
                    string interest = inputLower.Contains("i am interested in")
                        ? inputLower.Split("i am interested in")[1].Trim()
                        : inputLower.Split("i like")[1].Trim();

                    interest = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(interest);
                    lblInterest.Text = "Interest: " + interest;
                    return;
                }
                catch { }
            }

            if (inputLower.Contains("scam"))
                lblInterest.Text = "Interest: Scams";
            else if (inputLower.Contains("phishing"))
                lblInterest.Text = "Interest: Phishing";
            else if (inputLower.Contains("password"))
                lblInterest.Text = "Interest: Passwords";
            else if (inputLower.Contains("malware"))
                lblInterest.Text = "Interest: Malware";
            else if (inputLower.Contains("privacy"))
                lblInterest.Text = "Interest: Privacy";
            else if (inputLower.Contains("2fa"))
                lblInterest.Text = "Interest: Two-Factor Authentication";
        }
    }
}
