using AutoMapper;
using SportSkin.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSkin.Application.Services.Implementations
{
    public class ServiceCamiseta
    {
        private readonly IRepositoryCamiseta _repository;
        private readonly IMapper _mapper;

        public ServiceCamiseta(IRepositoryCamiseta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}
