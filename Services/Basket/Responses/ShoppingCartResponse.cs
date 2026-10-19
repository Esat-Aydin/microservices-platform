namespace Basket.Responses
{
    public record class ShoppingCartResponse
    {
        public string UserName { get; init; }
        public List<ShoppingCartItemResponse> Items { get; init; }
        
        //Default ctor
        public ShoppingCartResponse()
        {
            this.UserName = string.Empty;
            this.Items = new List<ShoppingCartItemResponse>();
        }
        //ctor with username only 
        public ShoppingCartResponse(string userName)
            :this(userName, new List<ShoppingCartItemResponse>())
        {
            
        }

        //Full Ctor
        public ShoppingCartResponse(string userName, List<ShoppingCartItemResponse> items)
        {
            this.UserName = userName;
            this.Items = items ?? new List<ShoppingCartItemResponse>();
        }
        public decimal TotalPrice => this.Items.Sum(item => item.Price * item.Quantity);
       
    }
}
