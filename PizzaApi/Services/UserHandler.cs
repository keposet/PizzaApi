using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Utilities;

namespace PizzaApi.Services
{
    public class UserHandler
    {
        private readonly UserContext _userContext;

        public UserHandler(UserContext userContext)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public ErrorMessage ValidateUserData(UserItem userItemDTO)
        {
            var status = new ErrorMessage { IsError = true };
            if (userItemDTO == null) 
            {
                status.Message = "Must Supply Data";
                return status;
            }
            if(userItemDTO.Name.IsNullOrEmpty())
            {
                status.Message = "Must Supply Name";
                return status;
            }
            if(userItemDTO.Role.IsNullOrEmpty())
            {
                status.Message = "Must Supply Role";
                return status;
            }
            if(userItemDTO.Credential.IsNullOrEmpty()) 
            {
                status.Message = "Must Supply Credential";
                return status;
            }
            if(!IsNameUnique(userItemDTO.Name))
            {
                status.Message = "Name Must Be Unique";
                return status;
            }

            status.IsError = false;
            return status;
        }

        public async Task<UserItemDTO> WriteUserItem(UserItem userItem)
        {
            if (_userContext.UserItems == null) throw new NullReferenceException(nameof(_userContext.UserItems));
            userItem.Id = GetNewRecordId();
            _userContext.UserItems.Add(userItem);
            await _userContext.SaveChangesAsync();

            return UserItemToDTO(userItem);
        }

        private bool IsNameUnique(string name)
        {
            if (_userContext.UserItems == null) throw new NullReferenceException(nameof(_userContext.UserItems));
            var x = _userContext.UserItems;
            return !_userContext.UserItems.Where(usr => usr.Name == name).Any(); //returns false when name is unique.
        }

        private long GetNewRecordId()
        {
            if (_userContext.UserItems == null) throw new NullReferenceException(nameof(_userContext.UserItems));
            return (long) _userContext.UserItems.ToArray().Length + 1;
        }

        private UserItemDTO UserItemToDTO(UserItem userItem) => new UserItemDTO
        {
            Id = userItem.Id,
            Name = userItem.Name,
            Role = userItem.Role,
        };
    }
}
