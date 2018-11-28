﻿using System.Collections.Generic;

namespace adc.Core.Constants {
    public static class Roles {
        public const string Admin = "Admin";
        public const string User = "User";

        public static List<string> AllRoles = new List<string> { Admin, User };
    }
}
