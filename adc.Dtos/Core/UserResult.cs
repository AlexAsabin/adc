using System.Collections.Generic;
using adc.Dtos.Base;
using adc.Dtos.Security;

namespace adc.Dtos.Core {
    public class UsersResult : BaseResult {
        public IEnumerable<UserModel> Items { get; set; }
    }
}
