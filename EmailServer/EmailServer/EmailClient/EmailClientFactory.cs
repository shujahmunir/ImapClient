﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmailInterfaces;

namespace EmailClient
{
    public static class EmailClientFactory
    {
        public static IEmailClient GetClient(EmailClientEnum type)
        {
            if (type == EmailClientEnum.IMAP) return new IMAP_Wrapper();
            if (type == EmailClientEnum.POP3) return new POP3_Wrapper();

            return null;
        }
    }
}
