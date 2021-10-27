
namespace WalletPlusIncAPI.Helpers.RequestFeatures
{
    public class RoomParameters : RequestParameters
    {
        public RoomParameters()
        {
            OrderBy = "name";
        }
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; } = int.MaxValue;
        public bool ValidPriceRange => MaxPrice > MinPrice;
        public string SearchQuery { get; set; }
    }
}
