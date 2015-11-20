using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using Study.WebServices.Models;

namespace Study.WebServices
{
    /// <summary>
    /// SampleService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class SampleService : System.Web.Services.WebService
    {
        protected readonly static ExeConfigurationFileMap FileMap = new ExeConfigurationFileMap() { ExeConfigFilename = "account.config" };
        protected readonly Configuration Config;
        protected readonly AccountSection AccountSection;
        public SampleService()
        {
            Config = ConfigurationManager.OpenMappedExeConfiguration(FileMap, ConfigurationUserLevel.None);
            if (! Config.HasFile)
            {
                throw new ArgumentException("系统未找到" + FileMap.ExeConfigFilename + "文件");
            }
            AccountSection = (AccountSection)Config.GetSection("accountConfig");
            if (AccountSection == null)
            {
                throw new ArgumentException(FileMap.ExeConfigFilename + "文件中未配置accountConfig节点");
            }
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public bool Create(AccountDto model)
        {
            AccountSection.Accounts.Add(model.AsModel());

            Config.Save(ConfigurationSaveMode.Modified);
            return true;
        }

        [WebMethod]
        public AccountDto[] GetAll()
        {
            return AccountSection.Accounts.Select(m => new AccountDto(m)).ToArray();
        }

        [WebMethod]
        public AccountDto GetById(Guid id)
        {
            return new AccountDto(AccountSection.Accounts[id.ToString()]);
        }

        [WebMethod]
        public bool Remove(AccountDto model)
        {
            var result = AccountSection.Accounts.Remove(model.AsModel());
            Config.Save(ConfigurationSaveMode.Modified);
            return result;
        }

        [WebMethod]
        public bool Update(AccountDto model)
        {
            var obj = AccountSection.Accounts[model.Id.ToString()];

            if (obj == null)
            {
                return false;
            }

            obj.Name = model.Name;
            obj.ModifiedAt = DateTime.Now;
            obj.Enable = model.Enable;
            obj.Password = model.Password;
            obj.UserName = model.UserName;
            obj.SortOrder = model.SortOrder;

            Config.Save(ConfigurationSaveMode.Modified);
            return true;
        }
    }
}
