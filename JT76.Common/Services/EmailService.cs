using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JT76.Common.ObjectExtensions;
using JT76.Data.Factories;

namespace JT76.Common.Services
{
    ////Production (TimedService): Send To File System
    //Create a timed service, to match with the servers DirectoryPickUpLocation and run this on the server
    //The timed service will send the emails, and this service will only place the files in the directory

    //Production: All email constants are in the ui web.config

    //interface
    public interface IEmailService
    {
        Task<bool[]> SendMail(string strTo, string strSubject, string strBody);
        Task<bool[]> SendMeMail(string strBody);
    }

    //ConfigEmailService
    //The email address being used in Jt76.Common\App.Config is public, and you may use this

    //For Gmail permissions on a different account (5.5.1 authentication error) log into your GMail account and enable less secure apps
    //https://www.google.com/settings/security/lesssecureapps to setup 

    public class ConfigEmailService : IEmailService
    {
        private const string StrMailToMeSubject = "FROM JT76";

        private readonly ConfigService _configService;
        private readonly FileService _fileService;

        public ConfigEmailService(ConfigService configService, FileService fileService)
        {
            _configService = configService;
            _fileService = fileService;
        }

        /// <summary>
        /// This async task can be called and forgot if not wanting to await the return
        /// calling with "var bResult = SendMail(...).Result;" will ensure this return will be handled
        /// check with "!bResult.Contains(false);"
        /// </summary>
        /// <param name="strTo">Email address to</param>
        /// <param name="strSubject">Email Subject</param>
        /// <param name="strBody">Email Body</param>
        /// <returns>An awaitable bool array</returns>
        public async Task<bool[]> SendMail(string strTo, string strSubject, string strBody)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var configSections = _configService.GetSectionGroup(ConfigService.JtAppSettings.GroupEmailSection.ToNameString());

            var taskList = new List<Task<bool>>();

            foreach (SmtpSection mailSetting in configSections.Sections)
            {
                var threadSafeMailSetting = mailSetting;

                if (mailSetting.DeliveryMethod.Equals((SmtpDeliveryMethod.Network)))
                {
                    taskList.Add(Task<bool>.Factory.StartNew(() => SendNetworkMail(threadSafeMailSetting, strTo, strSubject, strBody)));
                }
                else if (mailSetting.DeliveryMethod.Equals((SmtpDeliveryMethod.SpecifiedPickupDirectory)))
                {
                    taskList.Add(Task<bool>.Factory.StartNew(() => SendSpecifiedPickupMail(threadSafeMailSetting, strTo, strSubject, strBody)));
                }
                else
                    throw new NotImplementedException();
            }

            return await Task.WhenAll(taskList);
        }

        /// <summary>
        /// This async task can be called and forgot if not wanting to await the return
        /// calling with "var bResult = SendMail(...).Result;" will ensure this return will be handled
        /// check with "!bResult.Contains(false);"
        /// </summary>
        /// <param name="strBody">Email Body</param>
        /// <returns>An awaitable bool array</returns>
        public Task<bool[]> SendMeMail(string strBody)
        {
            var strMailToMeAddress = _configService.GetAppSetting(ConfigService.JtAppSettings.PrimaryDeveloperEmail.ToNameString());
            return SendMail(strMailToMeAddress, StrMailToMeSubject, strBody);
        }

        private static bool SendNetworkMail(SmtpSection mailSetting, string strTo, string strSubject, string strBody)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = mailSetting.Network.EnableSsl;
                smtpClient.Host = mailSetting.Network.Host;
                smtpClient.Port = mailSetting.Network.Port;
                smtpClient.UseDefaultCredentials = mailSetting.Network.DefaultCredentials;
                smtpClient.DeliveryMethod = mailSetting.DeliveryMethod;
                smtpClient.Credentials = new NetworkCredential(mailSetting.Network.UserName,
                    mailSetting.Network.Password);

                using (var mailMessage = new MailMessage(mailSetting.From, strTo, strSubject, strBody))
                {
                    smtpClient.Send(mailMessage);
                }
            }
            return true;
        }

        private bool SendSpecifiedPickupMail(SmtpSection mailSetting, string strTo, string strSubject, string strBody)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.DeliveryMethod = mailSetting.DeliveryMethod;
                smtpClient.PickupDirectoryLocation = _fileService.GetDirectoryFolderLocation(DirectoryFolders.Jt76Email);
                mailSetting.From = mailSetting.From;

                using (var mailMessage = new MailMessage(mailSetting.From, strTo, strSubject, strBody))
                {
                    smtpClient.Send(mailMessage);
                }
            }
            return true;
        }

        //works great if only sending one at a time
        //smtpClient can only be one instance per thread, so stacking SendAsync calls will err

        //smtpClient.SendCompleted += SendCompletedCallback;
        //smtpClient.SendAsync(mailMessage, "|To|" + strTo + |Subject|" + strSubject + "|Body|" + strBody);
    }
}