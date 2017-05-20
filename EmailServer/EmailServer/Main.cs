using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailClient;
using EmailInterfaces;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EmailServer
{
    class Main
    {
        private static System.IO.StreamWriter sw = null;
        private static string outputDirectory = Environment.CurrentDirectory;
        private static IEmailClient ImapClient = null;

        private string User_Email="";
        private string User_Pass = "";

        private string SMTP_Host = "";
        private string IMAP_Host = "";

        private string Host_Server = "";

        private string Protocol;


        private void ResolveServers_Host(string email) 
        {
            if (email.Contains("gmail")) 
            {
                SMTP_Host = "smtp.gmail.com";
                IMAP_Host = "imap.gmail.com";
            }
        }
        public static IEmailClient GetIEmailClient() 
        {
            return ImapClient;
        }
        public void Init(string email, string password,string protocol,string path)
        {
            string emailsContentFile;

            User_Email = email;
            User_Pass = password;
            Protocol = protocol;

            ResolveServers_Host(email);

            if(protocol.Equals("imap"))
            {
                 ImapClient = EmailClientFactory.GetClient(EmailClientEnum.IMAP);
            }
            else if (protocol.Equals("pop3"))
            {
                ImapClient = EmailClientFactory.GetClient(EmailClientEnum.POP3);
            }
            else 
            {
                 ImapClient = EmailClientFactory.GetClient(EmailClientEnum.IMAP);
            }

            if (path == null || path == "")
            {
                // path = "F:\\Projects\\temp\\emailresponse.txt";
                emailsContentFile = path + "\\emailresponse.txt";
                Console.WriteLine(path);
            }
            else 
            {
                emailsContentFile = outputDirectory + "\\emailresponse.txt";
            }

            if (System.IO.File.Exists(emailsContentFile))
                System.IO.File.Delete(emailsContentFile);

            sw = new System.IO.StreamWriter(System.IO.File.Create(emailsContentFile));

            ImapClient.Connect(IMAP_Host, email, password, 993, true);
            ImapClient.SetCurrentFolder("INBOX");
            // I assume that 5 is the last "ID" readed by my client. 
            // If I want to read all messages i use "ImapClient.LoadMessages();"
            //ImapClient.LoadRecentMessages(5);
            ImapClient.LoadMessages("6900", "*");
            // To read all my messages loaded:
            for (int i = 0; i < ImapClient.Messages.Count; i++)
            {
                IEmail msm = (IEmail)ImapClient.Messages[i];
                // Load all infos include attachments
                msm.LoadInfos();
                Console.WriteLine(msm.Date.ToString() + " - " + msm.From[0] + " - " +
                                  msm.Subject + " - " + msm.Attachments.Count);

                sw.WriteLine(msm.Date.ToString() + " - " + msm.From[0] + " - " +
                                  msm.Subject + " - " + msm.Attachments.Count);

                foreach (var attachment in msm.Attachments)
                {
                        
                        //ByteArrayToFile(attachment.Text,attachment.Body,path);
                }

            }

            if (sw != null)
            {
                sw.Close();
                sw.Dispose();
            }
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray,string outputDir)
        {
            if(outputDir == "" || outputDir == "")
            {
                outputDir = Environment.CurrentDirectory + "\\attachments";
            }
            bool exists = System.IO.Directory.Exists(outputDir);

            if (!exists)
                System.IO.Directory.CreateDirectory(outputDir);

            string file = outputDir+"\\"+fileName;

            try
            {
                using (var fs = new FileStream(file, System.IO.FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public void autoReply(IEmail msm)
        {
            MailMessage message = new MailMessage();

            MailAddress senderAdd = new MailAddress(msm.From[0]);
            message.Sender = senderAdd;
            message.Subject = msm.Subject;

            EmailReply reply = new EmailReply(User_Email, User_Pass, Host_Server);

            reply.CreateReply(message,"testing reply");

            
        }
        
    }
}
