using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Security.Encryption;
using Volo.Abp.TextTemplating;

namespace IntranetPortal.AppServices.EmailSending
{
    public class EmailSendingAppService : IntranetPortalAppService, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        protected IStringEncryptionService StringEncryptionService { get; }


        public EmailSendingAppService(IEmailSender emailSender, IStringEncryptionService stringEncryptionService)
        {
            _emailSender = emailSender;
            StringEncryptionService = stringEncryptionService;
        }

        public async Task SendMailAsync()
        {

            try
            {
                Logger.LogInformation($"SendMail requested ");
                Logger.LogDebug($"SendMail requested");

                await _emailSender.SendAsync(
                    "sanepalaaye@gmail.com", //this is the target email
                    "This is a test", // this is the subject of the mail
                    "This the the test body" //this is the body of the mail
                    );
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, nameof(SendMailAsync));
                throw new UserFriendlyException($"{ex}");
            }
        }


    }
}
