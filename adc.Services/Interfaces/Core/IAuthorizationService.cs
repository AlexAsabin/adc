using System.Threading.Tasks;
using adc.Dtos.Security;

namespace adc.Services.Interfaces.Core {
    public interface IAuthorizationService {
        Task<UserModel> Login(LoginModel model);
        Task Logout();
    }
}
