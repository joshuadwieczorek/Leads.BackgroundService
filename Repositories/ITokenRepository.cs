using Leads.BackgroundService.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leads.BackgroundService.Repositories
{
    public interface ITokenRepository
    {
        Task<Token> Create(Token token);
        Task<Token> Read(Guid tokenId);
        Task<IEnumerable<Token>> Read(int? id);
        Task<Token> Read(int? id, int? providerId);
        Task<Token> Update(Guid tokenId, DateTime expiresAt);
        Task<Token> Update(Guid tokenId, string configuration);
    }
}