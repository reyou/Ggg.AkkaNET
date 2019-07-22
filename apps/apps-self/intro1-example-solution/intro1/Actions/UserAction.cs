using System;
using intro1.ActionTypes;
using intro1.Entities;

namespace intro1.Actions
{
    public class UserAction
    {
        public UserActionTypes ActionType { get; set; }
        public TestUser User { get; set; }
        public Exception ExceptionToThrow { get; set; }

        public UserAction(UserActionTypes actionType)
        {
            this.ActionType = actionType;
        }
        public UserAction(UserActionTypes actionType, TestUser user)
        {
            this.ActionType = actionType;
            this.User = user;
        }


    }
}
