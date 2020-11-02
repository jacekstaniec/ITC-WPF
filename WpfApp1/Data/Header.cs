using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace WpfApp1.Data
{
    class Header
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Header_name { get; set; }
        public int Customer_id { get; set; }
        public SqlMoney Net { get; set; }
        public SqlMoney Gross { get; set; }
        //        public List<Detail> myDetails { get; set; } = new List<Detail>();

    }
}
