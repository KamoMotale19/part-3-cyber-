using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyber
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; } // e.g., "A) ...", "B) ..."
        public int CorrectIndex { get; set; } // 0-based
        public string Explanation { get; set; }
    }

    public class QuizManager
    {
        private List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;
        private bool _isActive = false;

        public QuizManager()
        {
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report as phishing", "D) Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Legitimate companies never ask for passwords via email. Report it!"
                },
                new QuizQuestion
                {
                    Question = "A strong password should be at least how many characters?",
                    Options = new List<string> { "A) 6", "B) 8", "C) 12", "D) 20" },
                    CorrectIndex = 2,
                    Explanation = "12+ characters is recommended; longer is harder to crack."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for all accounts is safe.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "If one account is breached, all accounts become vulnerable."
                },
                new QuizQuestion
                {
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "A) Two-Factor Authentication", "B) Two-File Access", "C) Trusted-First App", "D) Two-Frame Auth" },
                    CorrectIndex = 0,
                    Explanation = "2FA adds an extra layer of security with a second verification method."
                },
                new QuizQuestion
                {
                    Question = "Which is NOT a red flag for a phishing email?",
                    Options = new List<string> { "A) Generic greeting", "B) Professional logo", "C) Urgent language", "D) Request for personal info" },
                    CorrectIndex = 1,
                    Explanation = "Logos can be faked; always verify the sender address."
                },
                new QuizQuestion
                {
                    Question = "Before entering personal info on a website, check for:",
                    Options = new List<string> { "A) The website name", "B) HTTPS and padlock icon", "C) Professional design", "D) Many ads" },
                    CorrectIndex = 1,
                    Explanation = "HTTPS ensures the connection is encrypted. Always look for the padlock."
                },
                new QuizQuestion
                {
                    Question = "Which of these is a STRONG password?",
                    Options = new List<string> { "A) Password123", "B) MyBirthday1990", "C) Tr0p!cal$unset#2024", "D) 12345678" },
                    CorrectIndex = 2,
                    Explanation = "Mix uppercase, lowercase, numbers, and symbols; avoid personal info."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is safe for online banking.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi‑Fi is insecure; use a VPN for sensitive transactions."
                },
                new QuizQuestion
                {
                    Question = "Social engineering attacks often rely on:",
                    Options = new List<string> { "A) Building trust", "B) Creating urgency", "C) Impersonating authority", "D) All of the above" },
                    CorrectIndex = 3,
                    Explanation = "All these tactics are common. Always verify identities."
                },
                new QuizQuestion
                {
                    Question = "True or False: Antivirus alone is enough to protect you.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "You also need strong passwords, 2FA, safe browsing, and regular updates."
                },
                new QuizQuestion
                {
                    Question = "What should you do immediately if you suspect your account is hacked?",
                    Options = new List<string> { "A) Do nothing", "B) Change your password", "C) Delete your account", "D) Post about it on social media" },
                    CorrectIndex = 1,
                    Explanation = "Change your password immediately and enable 2FA."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should always install software updates promptly.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 0,
                    Explanation = "Updates fix security vulnerabilities. Install them as soon as possible."
                }
            };
        }

        public bool StartQuiz()
        {
            if (_isActive) return false;
            _isActive = true;
            _currentIndex = 0;
            _score = 0;
            return true;
        }

        public string GetCurrentQuestion()
        {
            if (!_isActive || _currentIndex >= _questions.Count) return null;
            var q = _questions[_currentIndex];
            string output = $"Q{_currentIndex + 1}/{_questions.Count}: {q.Question}\n";
            foreach (var opt in q.Options) output += opt + "\n";
            return output;
        }

        public string SubmitAnswer(string userInput)
        {
            if (!_isActive) return "No quiz in progress. Say 'Start quiz' to begin.";

            // Expect input like "A", "B", etc.
            string trimmed = userInput.Trim().ToUpper();
            if (trimmed.Length != 1 || trimmed[0] < 'A' || trimmed[0] > 'D')
                return "Please answer with A, B, C, or D.";

            int selected = trimmed[0] - 'A';
            var q = _questions[_currentIndex];
            bool correct = selected == q.CorrectIndex;
            if (correct) _score++;

            string feedback = correct ? " Correct! " : $" Wrong. The correct answer is {(char)('A' + q.CorrectIndex)}. ";
            feedback += q.Explanation;

            _currentIndex++;

            if (_currentIndex >= _questions.Count)
            {
                _isActive = false;
                float pct = (float)_score / _questions.Count * 100;
                feedback += $"\n\nQuiz finished! Score: {_score}/{_questions.Count} ({pct:F0}%)";
                if (pct >= 80) feedback += "\n Excellent! You're a cybersecurity pro!";
                else if (pct >= 60) feedback += "\n Good effort! Keep learning!";
                else feedback += "\n Keep studying – cybersecurity is important!";
                return feedback;
            }

            feedback += $"\n\nNext question:\n{GetCurrentQuestion()}";
            return feedback;
        }
        public double GetProgress()
        {
            if (_questions.Count == 0) return 0;
            return (double)_currentIndex / _questions.Count;
        }

        public bool IsActive => _isActive;
        public int GetScore() => _score;
        public int GetTotal() => _questions.Count;
    }
}