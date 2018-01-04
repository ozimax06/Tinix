using System.Diagnostics.SymbolStore;

namespace Tinix.Context
{
    public interface IAdminService
    {
        bool ValidateCredentials(string userName, string password);
    }
}