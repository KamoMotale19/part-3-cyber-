using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyber
{
    internal class Chatbot
    {
        private Random random = new Random();

        private string userName = "";
        private string userInterest = "";
        private string lastResponse = "";
        private string lastTopic = "";

        private enum ConversationState
        {
            WaitingForName,
            WaitingForInterest,
            Chatting
        }

        private ConversationState currentState = ConversationState.WaitingForName;
        private List<ResponseRule> rules;

        public Chatbot()
        {
            InitializeResponseRules();
        }

        private void InitializeResponseRules()
        {
            rules = new List<ResponseRule>()
            {
                new ResponseRule
                {
                    Keywords = new List<string> { "hello", "hi", "hey", "yo", "sup", "greetings" },
                    Responses = new List<string>
                    {
                        "Hey there! I'm here to help you understand how cyber threats work and how to stay safe online.",
                        "Hi! I can help you with passwords, phishing, scams, malware, privacy, and more. What would you like to know?",
                        "Hello! Ask me about cybersecurity topics like passwords, phishing, scams, or online safety."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "help", "what can you do", "confused", "what are you", "who are you" },
                    Responses = new List<string>
                    {
                        "I'm a cybersecurity chatbot designed to teach you how to stay safe online. I explain phishing, malware, scams, passwords, privacy, and more in simple terms.",
                        "I can help you learn about cybersecurity topics like passwords, phishing, scams, malware, privacy, and how to protect yourself online.",
                        "I'm here to guide you through cybersecurity concepts and real-life risks. Ask me about passwords, fake emails, hackers, or online safety."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "password", "pass", "pin", "login", "sign in", "log in", "account", "credentials", "my account" },
                    Responses = new List<string>
                    {
                        "A password is a secret string used to protect your account from unauthorized access. Never use the same password for multiple accounts!",
                        "Never reuse passwords across multiple accounts. If one gets hacked, attackers can access everything. Use different, strong passwords for each account.",
                        "Many accounts get hacked because of weak passwords. Always update your passwords regularly and keep them private. A strong password should be 12+ characters with numbers, symbols, and mixed case."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "strong password", "weak password", "how to create password", "password safety", "password tips" },
                    Responses = new List<string>
                    {
                        "A strong password should: be at least 12 characters long, mix uppercase and lowercase letters, include numbers and special symbols like exclamation marks and hashtags, and avoid personal information like birthdays or names.",
                        "Never use dictionary words or patterns like 123456 or qwerty. Hackers have tools that can crack these instantly.",
                        "Use a password manager to generate and store complex passwords. This way you only need to remember one master password."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is phishing", "define phishing", "phishing meaning" },
                    Responses = new List<string>
                    {
                        "Phishing is when attackers pretend to be trusted companies to steal your personal information. They often send fake emails or messages with malicious links.",
                        "Phishing is a type of cyber attack where criminals try to trick you into giving away sensitive data like passwords or banking details.",
                        "Phishing is a method used by hackers to deceive users into clicking harmful links or sharing private information."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "phishing attack", "fake email", "suspicious email", "email scam", "identify phishing", "spot phishing", "avoid phishing" },
                    Responses = new List<string>
                    {
                        "Phishing attacks often use urgent messages like your account will be locked to trick you into clicking fake links. Always verify the sender before clicking!",
                        "To avoid phishing: Check the sender's email address carefully, look for spelling errors, hover over links to see the real URL, never enter passwords on suspicious websites.",
                        "Be careful of emails that create panic or pressure you to act quickly - this is a common phishing tactic used by scammers."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is malware", "malware attack", "how i got hacked", "someone hacked my pc", "virus", "ransomware" },
                    Responses = new List<string>
                    {
                        "Malware is harmful software that damages your device or steals your data. It spreads through unsafe downloads, suspicious links, or infected attachments. Common types include viruses, ransomware, and spyware.",
                        "Viruses and ransomware can lock your files or crash your system. Always update your software and use antivirus protection to stay safe.",
                        "Hackers use malware to gain control of your device or steal personal information. Never download from unknown sources and always scan files before opening them."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is scam", "define scam", "scam meaning", "fraud" },
                    Responses = new List<string>
                    {
                        "A scam is when someone tricks you into giving away money or personal information. Scammers use fake offers, urgent messages, or impersonation to deceive you.",
                        "Scams are fake tricks used by criminals to steal money or sensitive information from people.",
                        "A scam is any dishonest scheme designed to deceive people for financial or personal gain."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "scammed", "money scam", "fake offer", "too good to be true", "investment scam", "job scam", "online scam", "whatsapp scam", "telegram scam" },
                    Responses = new List<string>
                    {
                        "Scammers often use fake opportunities or urgent messages to pressure you into acting quickly. Always take time to verify claims before sending money.",
                        "To avoid scams: Never trust offers that seem too good to be true, always verify before sending money, be suspicious of strangers asking for personal details.",
                        "Always be suspicious of strangers asking for money or personal details online. If something feels off, it probably is!"
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "website", "link", "url", "browser", "https", "download", "website safety" },
                    Responses = new List<string>
                    {
                        "Always check for HTTPS before entering personal information. The padlock icon means the connection is secure.",
                        "Avoid clicking unknown links. They can lead to fake websites designed to steal your data.",
                        "Only download from trusted websites. Malicious downloads can install hidden malware on your device."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is privacy", "define privacy", "privacy meaning" },
                    Responses = new List<string>
                    {
                        "Privacy means keeping your personal information safe and under your control. No one should access your data without permission.",
                        "Online privacy is about protecting your personal data from being shared or misused.",
                        "Privacy refers to how your personal information is collected, used, and protected online."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "data leak", "identity theft", "personal info", "bank details", "sharing info", "information stolen", "privacy risk" },
                    Responses = new List<string>
                    {
                        "Your personal data can be stolen if you share it on unsafe websites or apps. Be careful about what you share online!",
                        "To protect your privacy: Only share information with trusted sources, check app permissions, be selective about social media posts.",
                        "Be careful about what you post online because it can be used to track or identify you. Scammers often use public information to target people."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is threat", "define threat", "threat meaning", "cyber threat", "online threats" },
                    Responses = new List<string>
                    {
                        "A cyber threat is any danger that can harm your device, data, or online accounts.",
                        "Cyber threats include anything like hackers, malware, scams, or attacks designed to steal or damage information.",
                        "A threat in cybersecurity means anything that tries to exploit or harm a system or user online."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "cyber threats", "types of threats", "danger online", "hackers threat", "malware threat", "phishing threat", "risk online", "internet dangers" },
                    Responses = new List<string>
                    {
                        "Common cyber threats include: hackers trying to access accounts, phishing emails with fake links, malware that damages devices, scams that steal money.",
                        "These threats can steal passwords, damage devices, or trick users into losing money or sensitive information. Always stay vigilant!",
                        "The internet has many risks, so you must always be careful when clicking links, downloading files, or sharing information."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "what is 2fa", "define 2fa", "two factor authentication", "2fa meaning" },
                    Responses = new List<string>
                    {
                        "2FA (Two-Factor Authentication) is an extra security step used when logging into accounts. It adds a second layer of protection besides your password.",
                        "It requires two forms of verification to access your account securely.",
                        "2FA means you need two things to prove it's really you: something you know like a password and something you have like a phone or email."
                    }
                },

                new ResponseRule
                {
                    Keywords = new List<string> { "otp", "verification code", "security code", "login code", "authenticator", "sms code", "two step verification" },
                    Responses = new List<string>
                    {
                        "2FA protects your account even if someone gets your password. Enable it on important accounts!",
                        "Always enable 2FA on important accounts like email, banking, and social media for extra security.",
                        "Never share your verification codes with anyone - scammers often try to steal them by pretending to be customer support."
                    }
                }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you.";

            input = input.ToLower().Trim();

            if (currentState == ConversationState.WaitingForName)
            {
                if (input.Contains("my name is"))
                {
                    try
                    {
                        userName = input.Split("my name is")[1].Trim().Split(' ')[0];
                        currentState = ConversationState.WaitingForInterest;
                        return "Nice to meet you " + userName + "! What are you interested in learning about? You can say passwords, phishing, scams, malware, privacy, or anything related to cybersecurity.";
                    }
                    catch { }
                }
                else
                {
                    return "I didn't catch that. Could you please tell me your name? Just say 'My name is [your name]'";
                }
            }

            if (currentState == ConversationState.WaitingForInterest)
            {
                if (input.Contains("i am interested in") || input.Contains("i like"))
                {
                    try
                    {
                        if (input.Contains("i am interested in"))
                            userInterest = input.Split("i am interested in")[1].Trim();
                        else
                            userInterest = input.Split("i like")[1].Trim();

                        lastTopic = userInterest;
                        currentState = ConversationState.Chatting;

                        string response = "Great! I'm glad you're interested in " + userInterest + ". Here's what you should know:\n\n";
                        response += GetTopicInfo(userInterest);
                        return response;
                    }
                    catch { }
                }

                List<string> topics = new List<string> { "password", "phishing", "scam", "malware", "privacy", "threat", "2fa" };
                foreach (string topic in topics)
                {
                    if (input.Contains(topic))
                    {
                        userInterest = topic;
                        lastTopic = userInterest;
                        currentState = ConversationState.Chatting;

                        string response = "Great! I'm glad you're interested in " + userInterest + ". Here's what you should know:\n\n";
                        response += GetTopicInfo(userInterest);
                        return response;
                    }
                }

                return "Tell me what interests you! For example, you could say 'passwords', 'phishing', 'scams', 'malware', or 'privacy'";
            }

            if (DetectSentiment(input, out string sentiment, out string supportMessage))
            {
                return supportMessage;
            }

            if (input.Contains("tell me more") || input.Contains("another tip") || input.Contains("explain more") || input.Contains("more details"))
            {
                if (!string.IsNullOrEmpty(lastResponse))
                    return "Here's more: " + lastResponse;

                return "Can you tell me what topic you'd like more info on? I can help with passwords, phishing, scams, malware, privacy, and more!";
            }

            var rule = GetBestRule(input);

            if (rule != null)
            {
                string response = rule.Responses[random.Next(rule.Responses.Count)];
                lastResponse = response;
                lastTopic = rule.Keywords.First();

                if (!string.IsNullOrEmpty(userName))
                {
                    response = response + "\n\n- " + userName + ", stay safe online!";
                }

                return response;
            }

            return "I'm not sure I understand. Try asking about passwords, phishing, scams, malware, privacy, or online threats.";
        }

        private string GetTopicInfo(string topic)
        {
            if (topic.Contains("password"))
                return "A password is a secret string used to protect your account. Never use the same password for multiple accounts! A strong password should be 12 plus characters with numbers, symbols, and mixed case. Try to use unique passwords for every account you have.";

            if (topic.Contains("phishing"))
                return "Phishing is when attackers pretend to be trusted companies to steal your personal information. Always check the sender's email address carefully and never click suspicious links. Be wary of emails asking for personal details or passwords.";

            if (topic.Contains("scam"))
                return "A scam is when someone tricks you into giving away money or personal information. Never trust offers that seem too good to be true, and always verify before sending money. Be suspicious of strangers online asking for money or details.";

            if (topic.Contains("malware"))
                return "Malware is harmful software that damages your device or steals your data. It spreads through unsafe downloads and suspicious links. Always update your software and use antivirus protection to stay safe from viruses and ransomware.";

            if (topic.Contains("privacy"))
                return "Privacy means keeping your personal information safe and under your control. Only share information with trusted sources and be selective about what you post online. Be careful with what apps have access to on your device.";

            if (topic.Contains("threat"))
                return "A cyber threat is any danger that can harm your device, data, or online accounts. Common threats include hackers, malware, scams, and phishing attacks. Understanding threats helps you protect yourself better online.";

            if (topic.Contains("2fa"))
                return "2FA or Two-Factor Authentication is an extra security step when logging in. It requires two forms of verification like your password and a code from your phone. Enable 2FA on important accounts like email and banking for better protection.";

            return "That's a great topic! Feel free to ask me specific questions about it.";
        }

        private bool DetectSentiment(string input, out string sentiment, out string response)
        {
            sentiment = "";
            response = "";

            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid"))
            {
                sentiment = "Worried";
                response = "It's completely understandable to feel that way. Cybersecurity threats are real, but I'm here to help.\n\nLet me share some practical tips to help you stay safe online.\n\nFor starters: Use strong unique passwords, enable 2FA on important accounts, and be careful with suspicious emails.";
                return true;
            }

            if (input.Contains("frustrated") || input.Contains("angry") || input.Contains("annoyed"))
            {
                sentiment = "Frustrated";
                response = "I understand - cybersecurity can be confusing and frustrating.\n\nDon't worry, I'll keep things simple and easy to understand.\n\nWhat specific topic frustrates you most? I can explain it step by step.";
                return true;
            }

            if (input.Contains("curious") || input.Contains("interested") || input.Contains("want to know"))
            {
                sentiment = "Curious";
                response = "Great curiosity! That's the best way to learn about cybersecurity.\n\nLet me share some interesting facts and practical tips you can use right away.";
                return true;
            }

            if (input.Contains("happy") || input.Contains("great") || input.Contains("excellent"))
            {
                sentiment = "Happy";
                response = "I'm glad you're in a good mood! Let's make the most of it by learning something new about staying safe online.\n\nWhat would you like to know?";
                return true;
            }

            if (input.Contains("sad") || input.Contains("bad") || input.Contains("upset"))
            {
                sentiment = "Sad";
                response = "I'm sorry you're feeling down. Remember, learning about cybersecurity empowers you to protect yourself.\n\nLet me help you feel more confident about online safety.";
                return true;
            }

            return false;
        }

        private ResponseRule GetBestRule(string input)
        {
            ResponseRule bestRule = null;
            int bestScore = 0;

            foreach (var rule in rules)
            {
                int score = 0;

                foreach (var keyword in rule.Keywords)
                {
                    if (input.Contains(keyword))
                    {
                        score += keyword.Length;
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestRule = rule;
                }
            }

            return bestScore > 0 ? bestRule : null;
        }

        public string GetUserName() => userName;
        public string GetUserInterest() => userInterest;
        public string GetLastTopic() => lastTopic;
    }

    internal class ResponseRule
    {
        public List<string> Keywords { get; set; }
        public List<string> Responses { get; set; }
    }
}