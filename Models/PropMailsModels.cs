using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace BookingConfirm.Models
{
    public class ImagesModels
    {
        public string prop { get; set; }
        public string url { get; set; }
        public string description { get; set; }

        public ImagesModels(string prop, string url, string description)
        {
            this.prop = prop;
            this.url = url;
            this.description = description;
        }
    }

    public interface IImagesRepository
    {
        IEnumerable<ImagesModels> getImages();
        ImagesModels getImageByProp(string prop);
        void saveImages(ImagesModels images);
        void editImages(ImagesModels images);
        void deleteImages(string prop);
    }

    public class ImagesRepository : IImagesRepository
    {
        private List<ImagesModels> allImages;
        private XDocument imagesData;

        public ImagesRepository()
        {
            allImages = new List<ImagesModels>();

            imagesData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Images.xml"));

            var images = from image in imagesData.Descendants("property")
                           select new ImagesModels((string)image.Element("name").Value,
                           image.Element("url").Value,
                           image.Element("description").Value);
            allImages.AddRange(images.ToList<ImagesModels>());
        }

        public IEnumerable<ImagesModels> getImages()
        {
            return allImages;
        }

        public ImagesModels getImageByProp(string prop)
        {
            return allImages.Find(item => item.prop == prop);
        }

        public void saveImages(ImagesModels Image)
        {
            imagesData.Root.Add(new XElement("property", new XElement("prop", Image.prop), new XElement("url", Image.url),
                new XElement("description", Image.description)));

            imagesData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Images.xml"));
        }

        // Delete Record
        public void deleteImages(string prop)
        {
            imagesData.Root.Elements("property").Where(i => (string)i.Element("prop") == prop).Remove();

            imagesData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Images.xml"));
        }

        // Edit Record
        public void editImages(ImagesModels Images)
        {
            XElement node = imagesData.Root.Elements("property").Where(i => (string)i.Element("prop") == Images.prop).FirstOrDefault();

            node.SetElementValue("customer", Images.url);
            node.SetElementValue("description", Images.description);

            imagesData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Images.xml"));
        }
    }
    
}