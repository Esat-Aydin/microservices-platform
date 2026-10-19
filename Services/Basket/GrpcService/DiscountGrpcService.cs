using Discount.Grpc.Protos;

namespace Basket.GrpcService
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoServiceClient;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient)
        {
            this._discountProtoServiceClient = discountProtoServiceClient;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        { 
            GetDiscountRequest discountRequest = new GetDiscountRequest { ProductName = productName };
            return await this._discountProtoServiceClient.GetDiscountAsync(discountRequest);
        }
    }
}
