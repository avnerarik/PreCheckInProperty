using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.Net.Mail;
using BookingConfirm.Models;
using BookingConfirm.Helpers;

namespace BookingConfirm.Controllers
{
    public class ClientController : BaseController
    {
        private IImagesRepository _repository;
        private IPropMailsRepository _propmailsrepository;
        public ClientController() : this(new ImagesRepository(), new PropMailsRepository())
        {
        }

        public ClientController(IImagesRepository repository, IPropMailsRepository propmailsrepository)
        {
            _repository = repository;
            _propmailsrepository = propmailsrepository;
        }

        //
        // POST: /Client/CheckBooking
        // [HttpPost]
        // [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        //public ActionResult BookingCheck()
        //{
        //    try
        //    {
        //        ClientViewModel model = TempData["model"] as ClientViewModel;

        //        string fname = model.fname;
        //        string lname = model.lname;
        //        string resvn = model.reservNumber;
        //        string pcode = model.propCode;

        //        #region Json Request

        //        var webAddr = "https://chartsintf.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
        //        httpWebRequest.ContentType = "text/json";
        //        httpWebRequest.Method = "POST";

        //        JavaScriptSerializer js = new JavaScriptSerializer();

        //        propIdent property = new propIdent();
        //        property.mesg = "ResvSumm";
        //        property.propcode = pcode;

        //        Requests jsonrequest = new Requests();
        //        jsonrequest.ident = property;
        //        jsonrequest.reqn = "10";
        //        jsonrequest.name = lname;
        //        jsonrequest.lbkg = resvn;

        //        Requests[] sendarray = { jsonrequest };

        //        jsonRequestModel array = new jsonRequestModel();
        //        array.requests = sendarray;

        //        string json = JsonConvert.SerializeObject(array);


        //        using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //        }

        //        HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

        //        #endregion

        //        string result = streamReader.ReadToEnd(); //in result is the end data.

        //        jsonResponseModel response = JsonConvert.DeserializeObject<jsonResponseModel>(result);

        //        //if success
        //        if (response.responses[0].status.success)
        //        {
        //            if (response.responses[0].resvs.Length > 0)
        //            {
        //                TempData["jsonResponse"] = response;
        //                return RedirectToAction("BookingData", "Client", new { pcode = pcode, indx = response.responses[0].resvs[0].indx });
        //            }
        //            else
        //            {
        //                TempData["model"] = model;
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //        else //if failure
        //        {
        //            TempData["model"] = model;
        //            return RedirectToAction("Index", "Home");
        //        }

        //    }
        //    catch (WebException wex)
        //    {
        //        ClientViewModel model = TempData["model"] as ClientViewModel;
        //        TempData["model"] = model;
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        public ActionResult BookingData(string pcode, string indx)
        {
            //ViewBag.pict = _repository.getImageByProp(propcode).url;

            try
            {
                #region Json Fetch
                jsonResponseModel response = TempData["jsonResponse"] as jsonResponseModel;

                var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                JavaScriptSerializer js = new JavaScriptSerializer();

                login logdetails = new login();
                logdetails.user = "MGR";
                logdetails.pasw = "";

                propIdent property = new propIdent();
                property.mesg = "ResvFetch";
                property.propcode = pcode;
                property.login = logdetails;

                Fetch jsonfetch = new Fetch();
                jsonfetch.ident = property;
                jsonfetch.indx = indx;

                Fetch[] sendarray = { jsonfetch };

                jsonFetchModel array = new jsonFetchModel();
                array.requests = sendarray;

                string json = JsonConvert.SerializeObject(array);

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                string result = streamReader.ReadToEnd(); //in result is the end data.
                jsonFecthResponseModel fetchresponse = JsonConvert.DeserializeObject<jsonFecthResponseModel>(result);

                var data = fetchresponse.responses[0];
                #endregion

                #region Viewbag data
                ViewBag.indx = indx;
                ViewBag.data = data;
                ViewBag.pcode = pcode;
                ViewBag.Genders = FormHelper.GetGenderList();
                ViewBag.DocTypes = FormHelper.GetDocTypesList();
                ViewBag.ArrvHours = FormHelper.GetArrivalHours();
                ViewBag.Salutation = FormHelper.RequestSalutation(pcode);
                ViewBag.PayMethods = FormHelper.RequestPaymentMethod(pcode, data.stay.paym);

                ViewBag.cardNumber = "";
                if (data.stay.card != null) {
                    int length = data.stay.card.numb.Length;
                    int tohide = length - 8;
                    string aux = data.stay.card.numb.Substring(0, 4);
                    for (int i = 0; i < tohide; i++)
                    {
                        aux += "*";
                    }
                    aux += data.stay.card.numb.Substring(length - 4, 4);
                    ViewBag.cardNumber = aux;
                }

                ViewBag.noCountry = false;
                string country = data.guest.addr.cnty;
                List<DocumentTypeModel> Countries = FormHelper.RequestCountry(pcode);
                if (Countries.Find(i => i.value.ToUpper() == country.ToUpper()) == null)
                {
                    ViewBag.noCountry = true;
                }
                ViewBag.country = country;
                ViewBag.Countries = Countries;

                if (data.stay.exflds.Length > 0)
                {
                    foreach (var item in data.stay.exflds)
                    {
                        if (item.numb == 65)
                        {
                            ViewBag.gender = item.value != "" ? item.value : "";
                        }
                    }
                }

                //if (pcode.ToUpper() == "MIJE")
                //{
                //    if (data.stay.exflds.Length > 0)
                //    {
                //        foreach (var item in data.stay.exflds)
                //        {
                //            if (item.numb == 77)
                //            {
                //                if (item.value != "")
                //                {
                //                    var temp = item.value.Split(',');
                //                    ViewBag.document = temp[0];
                //                    ViewBag.idnumber = temp[1];
                //                    ViewBag.expdate = temp[2];
                //                }
                //            }
                //            if (item.numb == 83)
                //            {
                //                ViewBag.nationality = item.value != "" ? item.value : "";
                //            }
                //            if (item.numb == 65)
                //            {
                //                ViewBag.gender = item.value != "" ? item.value : "";
                //            }
                //        }
                //    }
                //}
                #endregion

                return View();

            }
            catch (WebException ex)
            {
                ClientViewModel model = TempData["model"] as ClientViewModel;
                TempData["Model"] = model;
                return RedirectToAction("Index", "Home", new { prop = pcode });
            }
        }

        [HttpPost]
        public ActionResult BookingData(BookingViewModel model)
        {
            //ViewBag.pict = _repository.getImageByProp(model.property).url;

            try
            {
                /*
                if (!Request.IsLocal && !Request.IsSecureConnection)
                {
                    string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                    Response.Redirect(redirectUrl, false);
                    HttpContext.ApplicationInstance.CompleteRequest();
                }
                */

                if (!ModelState.IsValid)
                {
                    #region Json Fetch
                    var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                    httpWebRequest.ContentType = "text/json";
                    httpWebRequest.Method = "POST";

                    JavaScriptSerializer js = new JavaScriptSerializer();

                    login logdetails = new login();
                    logdetails.user = "MGR";
                    logdetails.pasw = "";

                    propIdent property = new propIdent();
                    property.mesg = "ResvFetch";
                    property.propcode = model.property;
                    property.login = logdetails;

                    Fetch jsonfetch = new Fetch();
                    jsonfetch.ident = property;
                    jsonfetch.indx = model.index;

                    Fetch[] sendarray = { jsonfetch };

                    jsonFetchModel array = new jsonFetchModel();
                    array.requests = sendarray;

                    string json = JsonConvert.SerializeObject(array);

                    using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                    string result = streamReader.ReadToEnd(); //in result is the end data.
                    jsonFecthResponseModel fetchresponse = JsonConvert.DeserializeObject<jsonFecthResponseModel>(result);

                    var data = fetchresponse.responses[0];
                    #endregion

                    #region Viewbag data
                    ViewBag.indx = model.index;
                    ViewBag.pcode = model.property;
                    ViewBag.data = data;
                    ViewBag.DocTypes = FormHelper.GetDocTypesList();
                    ViewBag.ArrvHours = FormHelper.GetArrivalHours();
                    ViewBag.Genders = FormHelper.GetGenderList();
                    ViewBag.Salutation = FormHelper.RequestSalutation(model.property);

                    ViewBag.noCountry = false;
                    string country = data.guest.addr.cnty;
                    List<DocumentTypeModel> Countries = FormHelper.RequestCountry(model.property);
                    if (Countries.Find(i => i.value == country) == null)
                    {
                        ViewBag.noCountry = true;
                    }
                    ViewBag.country = country;
                    ViewBag.Countries = Countries;

                    //if (model.property.ToUpper() == "MIJE")
                    //{
                    if (data.stay.exflds.Length > 0)
                    {
                        foreach (var item in data.stay.exflds)
                        {
                            if (item.numb == 77)
                            {
                                if (item.value != "")
                                {
                                    var temp = item.value.Split(',');
                                    ViewBag.document = temp[0];
                                    ViewBag.idnumber = temp[1];
                                    ViewBag.expdate = temp[2];
                                }
                            }
                            if (item.numb == 83)
                            {
                                ViewBag.nationality = item.value != "" ? item.value : "";
                            }
                            if (item.numb == 65)
                            {
                                ViewBag.gender = item.value != "" ? item.value : "";
                            }
                        }
                    }
                    //}
                    #endregion

                    ModelState.AddModelError("", "Invalid attempt, try again.");
                    return View(model);
                }
                else
                {
                    TempData["Model"] = model;
                    return RedirectToAction("BookingConfirm", "Client");
                }

            }
            catch (WebException ex)
            {
                ClientViewModel temp = TempData["model"] as ClientViewModel;
                TempData["Model"] = temp;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult BookingConfirm()
        {
            BookingViewModel model = TempData["Model"] as BookingViewModel;
            try
            {
                //ViewBag.pict = _repository.getImageByProp(model.property).url;

                #region Json Update
                var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                JavaScriptSerializer js = new JavaScriptSerializer();

                login logdetails = new login();
                logdetails.user = "MGR";
                logdetails.pasw = "";

                propIdent requestdata = new propIdent();
                requestdata.mesg = "ResvUpdate";
                requestdata.propcode = model.property;
                requestdata.login = logdetails;

                Update jsonupdate = new Update();

                guest g = new guest();
                g.naml = model.lastName;
                g.namf = model.firstName;
                g.comp = model.company;
                g.emai = model.email;
                g.cell = model.cellnumber;
                g.phon = model.phonenumber;
                g.secu = model.secu;

                addr a = new addr();
                a.line1 = model.address1;
                a.line2 = model.address2;
                a.city = model.city;
                a.posc = model.postcode;
                a.stat = model.state;
                a.cnty = model.country;
                g.addr = a;

                stay s = new stay();
                s.arrt = model.timearrival;

                List<exflds> ext = new List<exflds>();

                if (model.gender != null)
                {
                    exflds e = new exflds();
                    e.numb = 65;
                    e.value = model.gender;
                    ext.Add(e);
                }

                if (ext.Count > 0)
                {
                    s.exflds = ext.ToArray();
                }

                //if (false) {
                //    List<exflds> ext = new List<exflds>();

                //    if (model.idnumber != null)
                //    {
                //        exflds e = new exflds();
                //        e.numb = 77;
                //        e.value = model.document + "," + model.idnumber + "," + model.expdate;
                //        ext.Add(e);
                //    }

                //    if (model.nationality != null)
                //    {
                //        exflds n = new exflds();
                //        n.numb = 83;
                //        n.value = model.nationality;
                //        ext.Add(n);
                //    }

                //    if (ext.Count > 0) {
                //        s.exflds = ext.ToArray();
                //    }
                //}

                jsonupdate.ident = requestdata;
                jsonupdate.indx = model.index;
                jsonupdate.guest = g;
                jsonupdate.stay = s;

                Update[] sendarray = { jsonupdate };

                jsonUpdateModel array = new jsonUpdateModel();
                array.requests = sendarray;

                string json = JsonConvert.SerializeObject(array);

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                string result = streamReader.ReadToEnd(); //in result is the end data.
                jsonUpdateResponseModel response = JsonConvert.DeserializeObject<jsonUpdateResponseModel>(result);
                #endregion

                if (response.responses[0].status.error.code == 0)
                {
                    #region LogFile
                    //DATE,TIME,PROPERTY,NAME,RESNUMBER

                    var dateAsString = System.DateTime.Now.ToString();

                    string loginfo = "Date: " + dateAsString + ", Property: " + model.property + ", Name: " + model.firstName + " "
                        + model.lastName + ", Reservation: " + model.bookingNumber + ";" + Environment.NewLine
                        + "-------------------" + Environment.NewLine;

                    logdata(loginfo);
                    #endregion

                    #region eMail
                    TempData["model"] = model;
                    sendMail();
                    #endregion

                    return View();
                }
                else
                {
                    logdata(response.responses[0].status.error.shorttext);
                    ClientViewModel m = TempData["model"] as ClientViewModel;
                    TempData["model"] = m;
                    return RedirectToAction("BookingData", "Client", new { pcode = model.property, indx = model.index });
                }
            }
            catch (WebException wex)
            {
                logdata(wex.InnerException.Message);
                ClientViewModel m = TempData["model"] as ClientViewModel;
                TempData["model"] = m;
                return RedirectToAction("BookingData", "Client", new { pcode = model.property, indx = model.index });
            }
        }

        public void logdata(string loginfo)
        {
            var dataFile = Server.MapPath("~/App_Data/Log.txt");

            if (System.IO.File.Exists(dataFile))
            { System.IO.File.AppendAllText(dataFile, loginfo); }
            else
            { System.IO.File.WriteAllText(dataFile, loginfo); }
        }

        #region Email
        public string PopulateBody(string fullName, string arrival, string departure, string roomType, string nofPeople, string bookDay, string property)
        {
            string body = string.Empty;

            string path = "~/App_Data/emailTemplate/" + property + "Conftemplate.html";

            using (StreamReader reader = new StreamReader(Server.MapPath(path)))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{fullName}", fullName);
            body = body.Replace("{arrival}", arrival);
            body = body.Replace("{departure}", departure);
            body = body.Replace("{roomType}", roomType);
            body = body.Replace("{nofPeople}", nofPeople);
            body = body.Replace("{bookDay}", bookDay);
            return body;
        }

        public void sendMail()
        {
            try
            {
                BookingViewModel model = TempData["model"] as BookingViewModel;

                string fullName = model.firstName + ' ' + model.lastName;
                string arrival = model.arrivalDate;
                string departure = model.departureDate;
                string roomType = model.roomType;
                string nofPeople = (model.adults + model.childs).ToString();
                string bookingDate = model.bookingDate;
                string property = model.property;

                string path = "~/App_Data/emailTemplate/" + property + "Conftemplate.html";

                if (System.IO.File.Exists(path))
                {
                    string template = PopulateBody(fullName, arrival, departure, roomType, nofPeople, bookingDate, property);

                    var b = _propmailsrepository.getMailByProp(property);

                    string mail = b.email;
                    string pass = b.pass;

                    var message = new MailMessage();
                    message.From = new MailAddress(mail);
                    message.To.Add(new MailAddress(model.email));
                    message.Subject = "Your reservation has been confirmed";
                    message.Body = template;
                    message.IsBodyHtml = true;

                    SmtpClient smtpMailObj = new SmtpClient()
                    {
                        Credentials = new NetworkCredential(mail, pass),
                        EnableSsl = true
                    };
                    //eg:localhost, 192.168.0.x, replace with your server nam
                    smtpMailObj.Host = "smtp.gmail.com";
                    smtpMailObj.Port = 587;
                }
                //smtpMailObj.Send(message);
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }
        }
        #endregion

    }
}