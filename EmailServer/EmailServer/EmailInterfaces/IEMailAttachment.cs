using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailInterfaces
{
    public interface IEMailAttachment
    {
        String Text { get; set; }
        Byte[] Body { get; set; }
    }
}
