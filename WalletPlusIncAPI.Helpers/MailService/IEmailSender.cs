using System.Threading.Tasks;
using WalletPlusIncAPI.Helpers.MailService;

namespace WalletManagementAPI.Helper.MailService
{
    public interface IEmailSender
    {
        Task SendMyEmailAsync (EmailMessage meesage);

    }
}
