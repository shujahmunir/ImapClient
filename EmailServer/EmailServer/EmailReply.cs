using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using EmailClient;


namespace EmailServer
{
    class EmailReply
    {

        /*
        private const string imapHost = "<HOST>"; // e.g. imap.gmail.com
        private const string imapUser = "<USERNAME>";
        private const string imapPassword = "<PASSWORD>";

        */
        private string SMTP_Host = "<HOST>"; // e.g. smtp.gmail.com
        private string User_Email = "";
        private string User_Pass = "";

        private string senderName = "<DISPLAY NAME>";

        public EmailReply(string email, string password, string host)
        {
            User_Email = email;
            User_Pass = password;

            if (host == "gmail") 
            {
                SMTP_Host = "smtp.gmail.com";
            }
        }

        public MailMessage CreateReply(MailMessage source, string message)
        {
            MailMessage reply = new MailMessage(new MailAddress(User_Email, "Sender"), source.From);

            // Get message id and add 'In-Reply-To' header
            string id = source.Headers["Message-ID"];
            reply.Headers.Add("In-Reply-To", id);

            // Try to get 'References' header from the source and add it to the reply
            string references = source.Headers["References"];

            if (!string.IsNullOrEmpty(references))
                references += ' ';

            reply.Headers.Add("References", references + id);

            // Add subject
            if (!source.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                reply.Subject = "Re: ";

            reply.Subject += source.Subject;


            reply.Body = message;
            reply.IsBodyHtml = source.IsBodyHtml;

            return reply;
        }

        public void SendReplies(IEnumerable<MailMessage> replies)
        {
            using (SmtpClient client = new SmtpClient(SMTP_Host, 587))
            {
                // Set SMTP client properties
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(User_Email, User_Pass);

                // Send
                bool retry = true;
                foreach (MailMessage msg in replies)
                {
                    try
                    {
                        client.Send(msg);
                        retry = true;
                    }
                    catch (Exception ex)
                    {
                        if (!retry)
                        {
                            Console.WriteLine("Failed to send email reply to " + msg.To.ToString() + '.');
                            Console.WriteLine("Exception: " + ex.Message);
                            return;
                        }

                        retry = false;
                    }
                    finally
                    {
                        msg.Dispose();
                    }
                }

                Console.WriteLine("All email replies successfully sent.");
            }
        }

        public void Execute(string[] args)
        {
            // Download unread messages from the server
            IEnumerable<MailMessage> messages = null;// GetMessages();
            if (messages != null)
            {
                Console.WriteLine(messages.Count().ToString() + " new email message(s).");

                // Create message replies
                List<MailMessage> replies = new List<MailMessage>();
                foreach (MailMessage msg in messages)
                {

                    replies.Add(CreateReply(msg, "test"));
                    msg.Dispose();
                }

                // Send replies
                SendReplies(replies);
            }
            else
            {
                Console.WriteLine("No new email messages.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
