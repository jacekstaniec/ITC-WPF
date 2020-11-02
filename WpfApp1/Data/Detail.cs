using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;

namespace WpfApp1.Data
{
    public class Detail
    {
        public int Id { get; set; }
        public int Header_id { get; set; }
        public string Article_name { get; set; }
        public int Quantity { get; set; }
        public SqlMoney Net { get; set; }
        public SqlMoney Gross { get; set; }
    }
}
