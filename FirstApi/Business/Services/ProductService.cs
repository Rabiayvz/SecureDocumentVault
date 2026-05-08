using FirstApi.Dtos;

namespace FirstApi.Services
{
    public class ProductService
    {
        private static List<ProductDto> _products = new List<ProductDto>();

        public List<ProductDto> GetProducts()
        {
            return _products;
        }

        public void AddProduct(ProductDto dto)
        {
            _products.Add(dto);
        }
    }
}