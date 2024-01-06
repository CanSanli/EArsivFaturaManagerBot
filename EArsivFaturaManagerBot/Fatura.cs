using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EArsivFaturaManagerBot
{
    class Fatura
    {
        public List<FaturaHatti> FaturaHatlari { get; set; }
        public string tevkifatTutarı { get; set; }
        public string faturaNo { get; set; }
        public string faturaTarihi { get; set; }
        public string faturaTipi { get; set; }
        public string supplierAd { get; set; }
        public string supplierSoyad { get; set; }
        public string supplierTcKimlikNo { get; set; }
        public string customerCompanyName { get; set; }
        public string customerVKN { get; set; }
        public string taxInclusiveAmount { get; set; }
        public string payableAmount { get; set; }
        public string kdvUcreti { get; set; }
        public string kdvDahilIslemUcreti { get; set; }
        public string kdvTevkifatUcreti { get; set; }
        public string hesaplananKDV { get; set; }
        public string toplamUcret { get; set; }
    }
}
