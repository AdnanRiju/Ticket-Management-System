using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using CompanyManagement.Models;
using CompanyManagement.Services;
using CompanyManagement.ViewModel;
using CompanyManagement.Constrains;
using CompanyManagement.Helper;

namespace MiniErp.Service.Services
{
    public class UserService : GenericService<User>
    {
        private readonly CMScontext _context;
        public UserService(CMScontext context) : base(context)
        {
            _context = context;
        }

        public VmUser? LoggedInUser(string username, string password)
        {
            try
            {
                string whereclause = $@" where u.[UserName]='{username}' and u.[Password]='{password}' and u.ActiveStatus != {(int)PropertyConstant.Deleted}";
                string sql = $@"SELECT u.*, ur.RoleName, ur.RoleType, b.BranchName, b.IsMasterBranch, l.LedgerName FROM [dbo].[User] u
                                inner join UserRole ur on ur.Id = u.UserRoleId
                                inner join Branch b on b.Id = u.BranchId 
                                left join Ledger l on l.Id = u.LedgerId {whereclause}";
                using (var connection = new SqlConnection(AppSetting.ConnectionString))
                {
                    return connection.Query<VmUser>(sql).FirstOrDefault();
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<User?> LoggedInUserAsync(string username, string password)
        {
            try
            {
                return await _context.User.AsNoTracking().Where(i => i.UserName == username && i.UserPassword == password && i.ActiveStatus == Convert.ToInt32(PropertyConstant.Active)).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<VmUser> GetVmUsers()
        {
            try
            {
                string sql = $@"SELECT u.[Id], u.[UserName], u.[Password], u.[UserTitle], u.[UserRoleId], ur.RoleName, ur.RoleType, u.[LastLoginTime], u.[LoginAttempt], u.[BranchId], b.BranchName, u.[LogoutTime], u.[AdminAccess], u.[ActiveStatus] FROM [dbo].[User] u
                    inner join UserRole ur on ur.Id = u.UserRoleId
                    inner join Branch b on b.Id = u.BranchId
                    where u.ActiveStatus != {(int)PropertyConstant.Deleted} and u.UserRoleId > 1";
                using (var connection = new SqlConnection(AppSetting.ConnectionString))
                {
                    return connection.Query<VmUser>(sql).ToList();
                };
            }
            catch (Exception)
            {
                return new List<VmUser>();
            }
        }
        public void LogOutUser(User user)
        {
            try
            {
                if (user != null)
                {
                    Update(user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        public ResponseMessage AddUser(User user)
        {
            try
            {
                var userlist = _context.User.AsNoTracking().ToList();
                if (userlist.Any(i => i.UserName == user.UserName)) return AutoResponse.ExistMessage("User name alredy exist");
                Add(user);
                return AutoResponse.SuccessMessage("User Added");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage UpdateUser(User user)
        {
            try
            {

                var userlist = _context.User.AsNoTracking().ToList();
                var existuser = userlist.Where(i => i.Id == user.Id).FirstOrDefault();
                if (existuser == null) return AutoResponse.NotFoundMessage("User not found for update");
                if (userlist.Any(i => i.UserName == user.UserName && i.Id != user.Id)) return AutoResponse.NotFoundMessage("User name already taken.");
                existuser.UserRoleId = user.UserRoleId;
                existuser.UserTitle = user.UserTitle;
                existuser.UserName = user.UserName;
                existuser.AdminAccess = user.AdminAccess;
                existuser.ActiveStatus = PropertyConstant.Active;
                _context.ChangeTracker.Clear();
                _context.User.Update(existuser);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("User Updated");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage UpdatePassword(long userId, string newPassword)
        {
            try
            {
                var existuser = _context.User.AsNoTracking().Where(i => i.Id == userId).FirstOrDefault();
                if (existuser == null) return AutoResponse.NotFoundMessage("User not found for update");
                existuser.UserPassword = newPassword;
                _context.ChangeTracker.Clear();
                _context.User.Update(existuser);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage("User password updated. please login again.");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }

        public ResponseMessage ResetPassword(long userid)
        {
            try
            {
                var existuser = _context.User.Where(i => i.Id == userid).FirstOrDefault();
                if (existuser == null) return AutoResponse.NotFoundMessage("user not found for reset password");
                //existuser.UserPassword = StringEncryption.EncryptMd5(GlobalConstant.ResetPassword);
                existuser.ActiveStatus = (int)PropertyConstant.Active;
                _context.ChangeTracker.Clear();
                _context.User.Update(existuser);
                _context.SaveChanges();
                return AutoResponse.SuccessMessage($@"User password reset. Use {GlobalConstant.ResetPassword} for login");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }
        
        public ResponseMessage ChangeActiveStatus(long userid, int status)
        {
            try
            {
                var existuser = _context.User.Where(i => i.Id == userid).FirstOrDefault();
                if (existuser == null) return AutoResponse.NotFoundMessage("user not found for reset password");
                existuser.ActiveStatus = status;
                _context.User.Update(existuser);
                _context.SaveChanges();
                if (status == PropertyConstant.Blocked)
                {
                    return AutoResponse.SuccessMessage("User account blocked");
                }
                return AutoResponse.SuccessMessage("User account active");
            }
            catch (Exception e)
            {
                var exmsg = e.Message.ToString() + ((e.InnerException != null) ? " ; " + e.InnerException.Message.ToString() : "");
                return AutoResponse.ExceptionOccuredMessage(exmsg);
            }
        }
        

        public List<VmUser> GetCustomerUsers()
        {
            try
            {
                string sql = $@"SELECT u.[Id], u.[UserName], u.[Password], u.[UserTitle], u.[UserRoleId], ur.RoleName, ur.RoleType, u.[LastLoginTime], u.[LoginAttempt], u.[BranchId], b.BranchName, u.[LogoutTime], u.[AdminAccess], u.[ActiveStatus] FROM [dbo].[User] u
                    inner join UserRole ur on ur.Id = u.UserRoleId
                    inner join Branch b on b.Id = u.BranchId
                    where u.ActiveStatus != {(int)PropertyConstant.Deleted} and ur.RoleType = {RoleTypeConstant.Customer}";
                using (var connection = new SqlConnection(AppSetting.ConnectionString))
                {
                    return connection.Query<VmUser>(sql).ToList();
                };
            }
            catch (Exception)
            {
                return new List<VmUser>();
            }
        }
    }
}
