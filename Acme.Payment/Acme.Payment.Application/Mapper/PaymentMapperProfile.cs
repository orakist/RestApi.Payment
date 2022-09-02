using Acme.Payment.Application.Dtos;
using Acme.Payment.Domain.Entities;
using AutoMapper;

namespace Acme.Payment.Application.Mapper;

public class PaymentMapperProfile : Profile
{
    public PaymentMapperProfile()
    {
        CreateMap<Account, AccountDto>()
            .ForMember(x => x.Owner, map => map.Ignore())
            .ForMember(x => x.Transactions, map => map.Ignore());
        CreateMap<Account, AccountSimpleDto>();

        CreateMap<Customer, CustomerDto>()
            .ForMember(x => x.Accounts, map => map.Ignore());
        CreateMap<Customer, CustomerSimpleDto>();

        CreateMap<Transaction, TransactionDto>()
            .ForMember(x => x.Account, map => map.Ignore())
            .ForMember(x => x.Adjustments, map => map.Ignore());
        CreateMap<Transaction, TransactionSimpleDto>()
            .ForMember(x => x.AccountNumber, map => map.Ignore());

        CreateMap<Adjustment, AdjustmentDto>();
    }
}