using System;
using DGCore.Common;

namespace Models
{
    public class VCubeMastAccount
    {
        public string ACCOUNT { get; set; }
        [BO_LookupTable("SqlClient;initial catalog=dbOneSAP_DW;Pooling=false;Data Source=localhost;Integrated Security=SSPI;Connection Timeout=300", "select * from vcube_mastaltacc", "ALTACC")]
        public VCubeMastAltAcc ALTACC { get; set; }
        public string ACCOUNT_GROUP { get; set; }
        public bool? BALANCE_SHEET { get; set; }
        public string SHORT_TEXT { get; set; }
        public string LONG_TEXT { get; set; }
        public string SHORT_ALTACC { get; set; }
        public byte? COST_COMPONENT { get; set; }
        public DateTime? DATED { get; set; }
        public int? TEST { get; set; }

    }

    public class VCubeMastAltAcc
    {
        public string ALTACC { get; set; }
        public string SHORTNAME { get; set; }
        public string LONGNAME { get; set; }
    }

}
