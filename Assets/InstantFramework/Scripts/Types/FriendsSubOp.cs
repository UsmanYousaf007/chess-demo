using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class FriendsSubOp
    {
        public class SubOpType
        {
            public const string REMOVE = "remove";
            public const string REMOVE_RECENT = "removeRecent";
        }

        public List<string> friendIds;
        public string subOp;

        public FriendsSubOp() { }

        public FriendsSubOp(List<string> a_friendIds, string a_subOp)
        {
            friendIds = a_friendIds;
            subOp = a_subOp;
        }
    }
}

