using System.Data;

namespace WpfApp1.Models
{
    public struct Article
    {
        public Article(int _id, int _hid, string _name, int _quantity, float _net, float _gross)
        {
            Id = _id;
            Header_id = _hid;
            Article_name = _name;
            Quantity = _quantity;
            Net = _net;
            Gross = _gross;
        }
        public int Id { get; private set; }
        public int Header_id { get; set; }
        public string Article_name { get; set; }
        public int Quantity { get; set; }
        public float Net { get; set; }
        public float Gross { get; private set; }
    }
}