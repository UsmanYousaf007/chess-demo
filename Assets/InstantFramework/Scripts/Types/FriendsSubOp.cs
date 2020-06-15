using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class FriendsSubOp
    {
        public List<string> friendIds;
        public FriendsSubOpFlag subOpFlag;

        public FriendsSubOp() { }

        public FriendsSubOp(List<string> a_friendIds, FriendsSubOpFlag a_subOpFlag)
        {
            friendIds = a_friendIds;
            subOpFlag = a_subOpFlag;
        }
    }
}

