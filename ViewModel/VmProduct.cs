namespace CompanyManagement.ViewModel
{
    public class VmProduct
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int ProductTypeId { get; set; }
        public string Product_Type { get; set; }
        public int? ActiveStatus { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
