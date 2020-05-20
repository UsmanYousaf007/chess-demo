using System;

namespace CustomHelperClasses
{
    public class UserOptions
    {
        public string UserId { set; get; }
        //   public string GroupName { set; get; }

        public UserOptions(string userId)
        {
            this.UserId = userId;
        }
    }


};