namespace WalletPlusIncAPI.Models.Dtos.Currency
{
  
    public class RequestRoot
    {
        public bool Success { get; set; }

        public int Timestamp { get; set; }

        public string Base { get; set; }

        public string Date { get; set; }

        public Rates Rates { get; set; }
    }
}