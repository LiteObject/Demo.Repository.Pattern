using AutoMapper;
using Demo.Repository.Pattern.Domain;
using Demo.Repository.Pattern.DTO;

namespace Demo.Repository.Pattern.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            _ = CreateMap<Product, ProductDto>();
        }
    }
}
