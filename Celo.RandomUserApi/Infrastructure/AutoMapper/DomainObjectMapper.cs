using System;
using AutoMapper;

namespace Celo.RandomUserApi.Infrastructure.AutoMapper
{

    public interface IDomainObjectMapper
    {
        (string, TDomainObject) MapTo<TDomainObject>(object dto);

    }

    public class DomainObjectMapper : IDomainObjectMapper
    {
        private readonly IMapper _autoMapper;

        public DomainObjectMapper(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }

        public (string, TDomainObject) MapTo<TDomainObject>(object dto)
        {
            try
            {
                return (string.Empty, _autoMapper.Map<TDomainObject>(dto));
            }
            catch (Exception e)
            {
                return ($"{e.Message}", default);
            }
        }
    }

}