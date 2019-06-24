using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace BookingConfirm.Models
{
    public class PropMailsModels
    {
        public string name { get; set; }
        public string email { get; set; }
        public string pass { get; set; }

        public PropMailsModels(string name, string email, string pass)
        {
            this.name = name;
            this.email = email;
            this.pass = pass;
        }
    }

    public interface IPropMailsRepository
    {
        IEnumerable<PropMailsModels> getPropMails();
        PropMailsModels getMailByProp(string prop);
        void savePropMail(PropMailsModels email);
        void editPropMail(PropMailsModels email);
        void deletePropMail(string prop);
    }

    public class PropMailsRepository : IPropMailsRepository
    {
        private List<PropMailsModels> allPropMails;
        private XDocument propMailsData;

        public PropMailsRepository()
        {
            allPropMails = new List<PropMailsModels>();

            propMailsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/PropMails.xml"));

            var emails = from mail in propMailsData.Descendants("property")
                           select new PropMailsModels((string)
                           mail.Element("name").Value,
                           mail.Element("email").Value,
                           mail.Element("pass").Value);
            allPropMails.AddRange(emails.ToList<PropMailsModels>());
        }

        public IEnumerable<PropMailsModels> getPropMails()
        {
            return allPropMails;
        }

        public PropMailsModels getMailByProp(string prop)
        {
            return allPropMails.Find(item => item.name == prop);
        }

        public void savePropMail(PropMailsModels Propmail)
        {
            propMailsData.Root.Add(new XElement("property", 
                new XElement("name", Propmail.name), 
                new XElement("email", Propmail.email),
                new XElement("pass", Propmail.pass))
            );

            propMailsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/PropMails.xml"));
        }

        // Edit Record
        public void editPropMail(PropMailsModels Images)
        {
            XElement node = propMailsData.Root.Elements("property").Where(i => (string)i.Element("name") == Images.name).FirstOrDefault();

            node.SetElementValue("email", Images.email);
            node.SetElementValue("pass", Images.pass);

            propMailsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/PropMails.xml"));
        }

        // Delete Record
        public void deletePropMail(string prop)
        {
            propMailsData.Root.Elements("property").Where(i => (string)i.Element("name") == prop).Remove();

            propMailsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/PropMails.xml"));
        }
    }
    
}