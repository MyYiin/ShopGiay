namespace ShopGiay.Models
{
    public class CartItem
    {
        public int TonkhoId { get; set; }         // Id của tồn kho
        public Mathang MatHang { get; set; }      // Thông tin mặt hàng
        public string MauSac { get; set; }        // Tên màu
        public double KichCo { get; set; }        // Giá trị size
        public int SoLuong { get; set; }
        public int MaMs { get; set; }
        public int MaKc { get; set; }
    }
}
