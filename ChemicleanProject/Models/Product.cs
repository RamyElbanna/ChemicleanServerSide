using System;

namespace ChemicleanProject.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string SupplierName { get; set; }
        public string Password { get; set; }
        public byte[] Bytes { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
