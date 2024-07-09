using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Terranova_GraphClient
{
    public interface IExchangeClient
    {
        Task<User> GetUserAsync();
        Task<Message> GetOriginalEmailAsync(string mailboxAddress, string mailItemId);
        Task MoveEmailToJunkAsync(string mailboxAddress, string mailItemId);
        Task MoveEmailToDeletedItemsAsync(string mailboxAddress, string mailItemId);
        Task<MailFolder> GetJunkMailFolderAsync(string mailboxAddress);
        Task ForwardEmailAsAttachmentAsync(string mailboxAddress, string mailItemId, List<Models.Recipient> forwardAddreses, string messageHeaders, string subject, bool formatSubjectForOffice365Reporting, string originalSubject, bool attachEmailWithOriginalTitle, bool moveToJunk, string type, string comment);
        Task ForwardEmailAsync(string mailboxAddress,string mailItemId, List<Models.Recipient> forwardAddreses);

        Task<MailFolder> GetDeletedItemsFolderAsync(string mailboxAddress);
        Task SetReportedStatusAsync(string mailboxAddress, string mailItemId);
        Task<Message> GetPreviouslyReportedStatusAsync(string mailboxAddress, string mailItemId);
    }
}
