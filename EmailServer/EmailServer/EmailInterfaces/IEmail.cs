using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LumiSoft.Net;
using LumiSoft.Net.IMAP;

namespace EmailInterfaces
{
    public interface IEmail
    {
        List<String> From
        {
            get;
            set;
        }

        List<IEMailAttachment> Attachments
        {
            get;
            set;
        }

        String TextBody
        {
            get;
            set;
        }

        String UID
        {
            get;
            set;
        }

        Int32 SequenceNumber { get; set; }

        long Size
        {
            get;
            set;
        }

        DateTime Date
        {
            get;
            set;
        }

        String Subject
        {
            get;
            set;
        }

        IMAP_t_MsgFlags Flag
        {
            get;
            set;
        }
        void LoadInfos();
    }
}
