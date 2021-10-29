using AutoMapper;
using WalletPlusIncAPI.Models.Dtos.AppUser;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Models.Dtos.Funding;
using WalletPlusIncAPI.Models.Dtos.Transaction;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;


namespace WalletPlusIncAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // source => target
            //appUser
            
            CreateMap<AppUser, AppUserReadDto>()
                .ForMember(c => c.Name, opt =>
                    opt.MapFrom(x => string.Join(' ', x.FirstName, x.LastName)));
            CreateMap<AppUserReadDto, AppUser>();
            CreateMap<AppUser, AppUserUpdateDto>();
            CreateMap<AppUserUpdateDto, AppUser>();
            CreateMap<AppUserRegisterDto, AppUser>();


            //Transaction
            CreateMap<Transaction, TransactionReadDto>()
                .ForMember(c => c.Type, opt => 
                    opt.MapFrom(d => d.Type.ToString()));
            CreateMap<TransactionReadDto, Transaction>();
          
            //Funding
            CreateMap<Funding, FundingReadDto>();
            CreateMap<FundingReadDto, Funding>();
            CreateMap<Funding, ApproveFundingDto>();
            CreateMap<ApproveFundingDto,Funding>();

            //Currency
            CreateMap<Currency, CurrencyReadDto>();
            CreateMap<CurrencyReadDto, Currency>();
           

            //Wallet
            CreateMap<Wallet, WalletReadDto>()
                .ForMember(c => c.Type, opt => 
                    opt.MapFrom(d => d.WalletType.ToString()));
            CreateMap<WalletReadDto, Wallet>()
                .ForMember(c => c.WalletType, opt => 
                    opt.MapFrom(d => d.Type));
            CreateMap<WalletCreateDto, Wallet>();
            CreateMap<Wallet, WalletCreateDto>();
            CreateMap<WalletUpdateDto,Wallet>();
            CreateMap<Wallet, WalletUpdateDto>();

        }
    }
}
