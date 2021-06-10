using Leads.Domain.Models.v1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Services.v1
{
    public interface ITokenService
    {
        Task<TokenModel> Get(Guid tokenId);
        Task<IEnumerable<TokenModel>> Get(int id);
        Task<TokenModel> Get(int id, int leadProviderId);
        Task<TokenModel> Get(int id, string leadProviderName);
        Task<TokenModel> Post(TokenModel tokenModel);
        Task Put(Guid tokenId);
        Task<TokenModel> Put(Guid tokenId, TokenModel tokenModel);
    }
}