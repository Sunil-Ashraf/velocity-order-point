using Microsoft.EntityFrameworkCore;
using OrderPoint.Domain.Entities;

namespace OrderPoint.Services
{
    public interface IWholesalerService
    {
        Task<Wholesaler?> GetWholesalerByNameAsync(string name);

    }
  
}
