using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    public class NetworkAccess
    {
        public const int SERVER = 0;
        public const int ADMIN = 1;
        public const int MODERATOR = 2;
        public const int MEMBER = 3;
        public const int DEFAULT = 4;
    }

    public class NetworkRequest
    {
        public const int USER_LIST = 0;
    }

}