namespace WpfApp1.Models
{
    using System;

    public struct Header
    {
        public Header(int _id, DateTime _date, string _name, int _cid, float _net, float _gross)
        {
            Id = _id;
            Date = _date;
            Header_name = _name;
            Customer_id = _cid;
            Net = _net;
            Gross = _gross;
        }

        public int Id { get; private set; }
        public DateTime Date { get; set; }
        public string Header_name { get; set; }
        public int Customer_id { get; set; }
        public float Net { get; private set; }
        public float Gross { get; private set; }
    }
}