using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Terranova_GraphClient
{
    public class ExchangeClient : IExchangeClient
    {
        private readonly GraphServiceClient _graphServiceClient;
        private const string _JunkFolderDestinationId = "JunkEmail";
        private const string _DeletedItemsDestinationId = "DeletedItems";
        private const long _MaxFileSizeForSingleUpload = 3145728;
        private const int _BytesPerKiloByte = 1024;
        private const int _SliceSize = 320;
        private const int _MaxRetryCount = 3;
        public const string _SingleValueExtendedPropertyName = "String {00020329-0000-0000-c000-000000000046} Name IsReported";
        public const string _SingleValueExtendedPropertyValue = "True";

        public ExchangeClient(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> GetUserAsync()
        {
            return await _graphServiceClient.Me.Request().GetAsync();
        }

        public async Task<Message> GetOriginalEmailAsync(string mailboxAddress, string mailItemId)
        {
            return await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId]
                 .Request()
                 .Header("Prefer", "outlook.body-content-type=\"html\"")
                 .Select("subject, body, internetmessageheaders, parentFolderId")
                 .GetAsync();
        }

        public async Task MoveEmailToJunkAsync(string mailboxAddress, string mailItemId)
        {
            await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId]
                .Move(_JunkFolderDestinationId)
                .Request()
                .PostAsync();
        }

        public async Task<MailFolder> GetDeletedItemsFolderAsync(string mailboxAddress)
        {
            return await _graphServiceClient.Users[mailboxAddress].MailFolders["DeletedItems"]
                .Request()
                .Select("id")
                .GetAsync();
        }

        public async Task MoveEmailToDeletedItemsAsync(string mailboxAddress, string mailItemId)
        {
            await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId]
                .Move(_DeletedItemsDestinationId)
                .Request()
                .PostAsync();
        }

        public async Task<MailFolder> GetJunkMailFolderAsync(string mailboxAddress)
        {
            return await _graphServiceClient.Users[mailboxAddress].MailFolders["JunkEmail"]
                .Request()
                .Select("id")
                .GetAsync();
        }

        public static byte[] ToByteArray(Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }

        public async Task ForwardEmailAsAttachmentAsync(string mailboxAddress, string mailItemId, List<Models.Recipient> forwardAddreses, string messageHeaders, string subject, bool formatSubjectForOffice365Reporting, string originalSubject, bool attachEmailWithOriginalTitle, bool moveToJunk, string type, string comment)
        {
            if (string.IsNullOrEmpty(mailItemId))
            {
                throw new ArgumentNullException($"{nameof(mailItemId)} can not be null or empty");
            }
            if (forwardAddreses == null)
            {
                throw new ArgumentNullException($"{nameof(forwardAddreses)} can not be null");
            }

            var content = await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId].Content.Request().WithMaxRetry(_MaxRetryCount).GetAsync();
            var message = new Message
            {
                Subject = formatSubjectForOffice365Reporting ? $"3|{subject}" : subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = $"Type: {type}{Environment.NewLine}{Environment.NewLine}Comment: {comment}{Environment.NewLine}{Environment.NewLine}{messageHeaders}"
                },
                ToRecipients = forwardAddreses.Where(x => !x.IsBcc).Select(x => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = x.EmailAddress
                    }
                }).ToList(),
                BccRecipients = forwardAddreses.Where(x => x.IsBcc).Select(x => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = x.EmailAddress
                    }
                }).ToList()
            };

            if (content.Length < _MaxFileSizeForSingleUpload)
            {
                //upload attachment to the message using a single request if mime content is less than 3 mb
                var page = new MessageAttachmentsCollectionPage
                {
                    AdditionalData = new Dictionary<string, object>()
                };
                page.Add(new FileAttachment
                {
                    ODataType = "#microsoft.graph.fileAttachment",
                    Name = attachEmailWithOriginalTitle ? $"{originalSubject}.eml" : "Spam Report.eml",
                    ContentBytes = ToByteArray(content)
                });
                message.Attachments = page;
                await _graphServiceClient.Me.SendMail(message, true).Request().PostAsync();
            }
            else
            {
                //if mime content is more than 3 mb then file is sliced into smaller pieces and upload a single slice
                //create draft message
                var draftItem = await _graphServiceClient.Me.Messages.Request().AddAsync(message);
                //create an upload session
                var attachmentItem = new AttachmentItem
                {
                    AttachmentType = AttachmentType.File,
                    Name = attachEmailWithOriginalTitle ? $"{originalSubject}.eml" : "Spam Report.eml",
                    Size = content.Length
                };
                var uploadSession = await _graphServiceClient.Me.Messages[draftItem.Id]
                                                    .Attachments.CreateUploadSession(attachmentItem)
                                                    .Request().PostAsync();

                if (await UploadAttachmentContentAsync(content, uploadSession))
                {
                    await _graphServiceClient.Me.Messages[draftItem.Id].Send().Request().PostAsync();
                }
            }
        }

        public async Task ForwardEmailAsync(string mailboxAddress, string mailItemId, List<Models.Recipient> forwardAddreses)
        {
            if (string.IsNullOrEmpty(mailItemId))
            {
                throw new ArgumentNullException($"{nameof(mailItemId)} can not be null or empty");
            }
            if (forwardAddreses == null)
            {
                throw new ArgumentNullException($"{nameof(forwardAddreses)} can not be null");
            }

            var message = new Message
            {
                ToRecipients = forwardAddreses.Where(x => !x.IsBcc).Select(x => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = x.EmailAddress
                    }
                }).ToList(),

                BccRecipients = forwardAddreses.Where(x => x.IsBcc).Select(x => new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = x.EmailAddress
                    }
                }).ToList()
            };
            await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId]
                .Forward(null, message, string.Empty)
                .Request()
                .PostAsync();
        }

        private static async Task<bool> UploadAttachmentContentAsync(Stream content, UploadSession uploadSession)
        {
            // Max slice size must be a multiple of 320 KiB
            int maxSliceSize = _SliceSize * _BytesPerKiloByte;
            var fileUploadTask = new LargeFileUploadTask<Attachment>(uploadSession, content, maxSliceSize);
            long uploadedBytes = 0;
            // Create a callback that is invoked after each slice is uploaded
            IProgress<long> progress = new Progress<long>(prog =>
            {
                uploadedBytes = uploadedBytes + prog;
            });
            try
            {
                // Upload the file
                var uploadResult = await fileUploadTask.UploadAsync(progress);
                if (uploadResult.UploadSucceeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var contentInMB = Math.Ceiling(Convert.ToDecimal(content.Length / (_BytesPerKiloByte * _BytesPerKiloByte)));
                var uploadedBytesInMB = Math.Ceiling(Convert.ToDecimal(uploadedBytes / (_BytesPerKiloByte * _BytesPerKiloByte)));
                throw new Exception($"eml file size to upload: {contentInMB}MB, Total uploaded: {uploadedBytesInMB}MB,  error: {ex.Message}");
            }
        }

        public async Task SetReportedStatusAsync(string mailboxAddress, string mailItemId)
        {
            var requestBody = new Message
            {
                SingleValueExtendedProperties = new MessageSingleValueExtendedPropertiesCollectionPage
                {
                    new SingleValueLegacyExtendedProperty
                    {
                        Id = _SingleValueExtendedPropertyName,
                        Value = _SingleValueExtendedPropertyValue,
                    },
                },
            };
            await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId].Request().UpdateAsync(requestBody);
        }

        public async Task<Message> GetPreviouslyReportedStatusAsync(string mailboxAddress, string mailItemId)
        {
            var expandQuery = $"singleValueExtendedProperties($filter=id eq '{_SingleValueExtendedPropertyName}')";
            return await _graphServiceClient.Users[mailboxAddress].Messages[mailItemId]
                 .Request()
                 .Expand(expandQuery)
                 .GetAsync();
        }
    }
}
