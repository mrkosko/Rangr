using System;
using System.Threading.Tasks;
using common_lib;

namespace App.Common
{
    public class LoginViewModel : ViewModelBase
    {
        public string user_mobile_number { get; set; }

        public string user_secret_code { get; set; }

        public async Task<bool> CreateUser()
        {
            //TODO: Validate mobile number here...

            var userID = await create_user_service
                                .Create(new CreateUserRequest() { 
                                            mobile_number = user_mobile_number
                                        });

            if (userID == null)
            {
                return false;
            }

            var user = new User()
            { 
                user_id = userID.user_id
            };

            sessionInstance.PersistCurrentUser(user);

            return true;
        }

        public async Task<bool> VerifyUser()
        {
            //TODO: Validate mobile number here...

            var userID = await verify_user
                                .execute(new VerifyUserRequest() { 
                                    user_id = user_mobile_number,
                                    secret_code = user_secret_code
                                });

            if (userID == null)
            {
                return false;
            }

            var user = new User() { 
                user_id = userID.user_id
            };

            sessionInstance.PersistCurrentUser(user);

            return true;
        }

        public LoginViewModel()
        {
            sessionInstance = Session.Current;

            create_user_service = new CreateUser();
            verify_user = new VerifyUser();
        }

        private CreateUser create_user_service;
        private VerifyUser verify_user;
        private Session sessionInstance;
    }
}

