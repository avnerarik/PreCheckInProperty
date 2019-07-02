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
                var bookingDetails = JsonRequestsHelper.RequestBookingDetails(pcode, indx);

                TempData["bookingDetails"] = bookingDetails;

                #region Viewbag data
                ViewBag.indx = indx;
                ViewBag.pcode = pcode;
                ViewBag.data = bookingDetails;
                ViewBag.Genders = FormHelper.GetGenderList();
                ViewBag.DocTypes = FormHelper.GetDocTypesList();
                ViewBag.ArrvHours = FormHelper.GetArrivalHours();
                ViewBag.Salutation = FormHelper.RequestSalutation(pcode);
                ViewBag.PayMethods = FormHelper.RequestPaymentMethod(pcode, bookingDetails.stay.paym);

                ViewBag.cardNumber = "";
                if (bookingDetails.stay.card != null)
                {
                    ViewBag.cardNumber = processCardNumbers(bookingDetails.stay.card.numb);
                }

                ViewBag.noCountry = false;
                string country = bookingDetails.guest.addr.cnty;
                List<DocumentTypeModel> Countries = FormHelper.RequestCountry(pcode);
                if (Countries.Find(i => i.value.ToUpper() == country.ToUpper()) == null)
                {
                    ViewBag.noCountry = true;
                }
                ViewBag.country = country;
                ViewBag.Countries = Countries;

                if (bookingDetails.stay.exflds.Length > 0)
                {
                    foreach (var item in bookingDetails.stay.exflds)
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
                if (!ModelState.IsValid)
                {
                    var bookingDetails = JsonRequestsHelper.RequestBookingDetails(model.property, model.index);

                    #region Viewbag data
                    ViewBag.indx = model.index;
                    ViewBag.pcode = model.property;
                    ViewBag.data = bookingDetails;
                    ViewBag.DocTypes = FormHelper.GetDocTypesList();
                    ViewBag.ArrvHours = FormHelper.GetArrivalHours();
                    ViewBag.Genders = FormHelper.GetGenderList();
                    ViewBag.Salutation = FormHelper.RequestSalutation(model.property);
                    ViewBag.PayMethods = FormHelper.RequestPaymentMethod(model.property, bookingDetails.stay.paym);

                    ViewBag.cardNumber = "";
                    if (bookingDetails.stay.card != null)
                    {
                        ViewBag.cardNumber = processCardNumbers(bookingDetails.stay.card.numb);
                    }

                    ViewBag.noCountry = false;
                    string country = bookingDetails.guest.addr.cnty;
                    List<DocumentTypeModel> Countries = FormHelper.RequestCountry(model.property);
                    if (Countries.Find(i => i.value == country) == null)
                    {
                        ViewBag.noCountry = true;
                    }
                    ViewBag.country = country;
                    ViewBag.Countries = Countries;

                    //if (model.property.ToUpper() == "MIJE")
                    //{
                    if (bookingDetails.stay.exflds.Length > 0)
                    {
                        foreach (var item in bookingDetails.stay.exflds)
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
                    return RedirectToAction("BookingConfirm", "Client", new { property = model.property, index = model.index });
                }

            }
            catch (WebException ex)
            {
                ClientViewModel temp = TempData["model"] as ClientViewModel;
                TempData["Model"] = temp;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult BookingConfirm(string property, string index)
        {
            BookingViewModel model = TempData["Model"] as BookingViewModel;
            
            ViewBag.indx = index;
            ViewBag.pcode = property;
            ViewBag.allowPayment = false;

            if (model == null) {
                return View();
            }

            try
            {
                //ViewBag.pict = _repository.getImageByProp(model.property).url;

                UpdateResponse response = JsonRequestsHelper.UpdateBookingDetails(model.property, model);
                
                if (response.status.error.code == 0)
                {
                    #region LogFile
                    var dateAsString = System.DateTime.Now.ToString();
                    string loginfo = "Date: " + dateAsString + ", Property: " + model.property + ", Name: " + model.firstName + " "
                        + model.lastName + ", Reservation: " + model.bookingNumber + ";" + Environment.NewLine
                        + "-------------------" + Environment.NewLine;
                    logdata(loginfo);
                    #endregion

                    TempData["Model"] = model;

                    #region eMail
                    sendMail();
                    #endregion

                    if (model.cardNumber!=null && model.cardNumber!="" && (Convert.ToDecimal(model.total)) > 0) { 
                        ViewBag.allowPayment = true;
                    }

                    return View();
                }
                else
                {
                    #region LogFile
                    var dateAsString = System.DateTime.Now.ToString();
                    string loginfo = "Date: " + dateAsString +
                        ", Property: " + model.property +
                        ", Name: " + model.firstName + " " + model.lastName +
                        ", Reservation: " + model.bookingNumber +
                        ", Status: " + response.status.error.shorttext +
                        ";" + Environment.NewLine
                        + "-------------------" + Environment.NewLine;
                    logdata(loginfo);
                    #endregion

                    ClientViewModel m = TempData["Model"] as ClientViewModel;
                    TempData["Model"] = m;
                    return RedirectToAction("BookingData", "Client", new { pcode = model.property, indx = model.index });
                }
            }
            catch (WebException wex)
            {
                #region LogFile
                var dateAsString = System.DateTime.Now.ToString();
                string loginfo = "Date: " + dateAsString +
                    ", Property: " + model.property +
                    ", Name: " + model.firstName + " " + model.lastName +
                    ", Reservation: " + model.bookingNumber +
                    ", Status: " + wex.InnerException.Message +
                    ";" + Environment.NewLine
                    + "-------------------" + Environment.NewLine;
                logdata(loginfo);
                #endregion

                ClientViewModel m = TempData["Model"] as ClientViewModel;
                TempData["Model"] = m;
                return RedirectToAction("BookingData", "Client", new { pcode = model.property, indx = model.index });
            }
        }
        
        public ActionResult PaymentData(string pcode, string indx)
        {
            //ViewBag.pict = _repository.getImageByProp(propcode).url;

            try
            {
                var bookingDetails = JsonRequestsHelper.RequestBookingDetails(pcode, indx);
                TempData["bookingDetails"] = bookingDetails;
                //FetchResponse bookingDetails = TempData["bookingDetails"] as FetchResponse;
                
                ViewBag.indx = indx;
                ViewBag.pcode = pcode;

                ViewBag.Payed = false;
                ViewBag.Error = false;

                ViewBag.data = bookingDetails;
                ViewBag.PayMethods = FormHelper.RequestPaymentMethod(pcode, bookingDetails.stay.paym);

                ViewBag.cardNumber = bookingDetails.stay.card.numb;

                //ViewBag.cardNumber = "";
                //if (bookingDetails.stay.card != null)
                //{
                //    ViewBag.cardNumber = processCardNumbers(bookingDetails.stay.card.numb);
                //}

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
        public ActionResult PaymentData(PaymentViewModel model)
        {
            ViewBag.Payed = false;
            ViewBag.Error = false;
            ViewBag.Message = "";

            //var cardtype = model.cardType;
            //var holderName = model.cardHolder;
            //var cardNumber = model.cardNumber;
            //var cardExpiry = model.cardEndDate;
            //var cardCode = model.cardCode;

            //ViewBag.Error = true;
            //ViewBag.Message = "Ok";
            //ViewBag.TransId = 01;
            //return View();

            string loginId = System.Configuration.ConfigurationManager.AppSettings["loginId"];
            string transKey = System.Configuration.ConfigurationManager.AppSettings["transKey"];

            FetchResponse bookingDetails = TempData["bookingDetails"] as FetchResponse;
            
            ViewBag.PayMethods = FormHelper.RequestPaymentMethod(model.property, bookingDetails.stay.paym);
            ViewBag.data = bookingDetails;

            var prepay = JsonRequestsHelper.PrePostTransactionDetails(model.property, bookingDetails);
            if (prepay.status.error.code == 0)
            {
                #region LogFile
                var dateAsString = System.DateTime.Now.ToString();
                string loginfo = "Date: " + dateAsString +
                    ", Property: " + model.property +
                    ", Name: " + model.firstName + " " + model.lastName +
                    ", Reservation: " + model.bookingNumber +
                    ", ChartsPrePost status: " + prepay.status.success +
                    ", ChartsPrePost amount: " + prepay.paym.amnt +
                    ", ChartsPrePost commision: " + prepay.paym.comm +
                ";" + Environment.NewLine
                + "-------------------" + Environment.NewLine;
                logdata(loginfo);
                #endregion

                Array result = ANetHelper.PostPay(loginId, transKey, bookingDetails, model, prepay.paym.amntnd);
                if (result.GetValue(0).ToString() == "1")
                {
                    #region LogFile
                    dateAsString = System.DateTime.Now.ToString();
                    loginfo = "Date: " + dateAsString +
                        ", Property: " + model.property +
                        ", Name: " + model.firstName + " " + model.lastName +
                        ", Reservation: " + model.bookingNumber +
                        ", AuthNetTrans transaction: " + result.GetValue(4).ToString() +
                        ", AuthNetTrans status: " + result.GetValue(0).ToString() +
                        ", AuthNetTrans message: " + result.GetValue(2).ToString() +
                        ", AuthNetTrans amount: " + prepay.paym.amntnd +
                    ";" + Environment.NewLine
                    + "-------------------" + Environment.NewLine;
                    logdata(loginfo);
                    #endregion

                    var pay = JsonRequestsHelper.PostTransactionDetails(model.property, bookingDetails, prepay);
                    if (pay.status.error.code == 0)
                    {
                        ViewBag.Payed = true;
                        ViewBag.Message = result.GetValue(2);
                        ViewBag.TransId = result.GetValue(4);

                        #region LogFile
                        dateAsString = System.DateTime.Now.ToString();
                        loginfo = "Date: " + dateAsString +
                            ", Property: " + model.property +
                            ", Name: " + model.firstName + " " + model.lastName +
                            ", Reservation: " + model.bookingNumber +
                            ", ChartsPostPay status: " + pay.status.success +
                            ", ChartsPostPay amount: " + prepay.paym.amnt +
                        ";" + Environment.NewLine
                        + "-------------------" + Environment.NewLine;
                        logdata(loginfo);
                        #endregion
                    }
                    else
                    {
                        ViewBag.Error = true;
                        ViewBag.Message = pay.status.error.shorttext;
                        ViewBag.TransId = result.GetValue(4);
                        Array voidResult = ANetHelper.Void(loginId, transKey, bookingDetails, result.GetValue(4).ToString());

                        #region LogFile
                        dateAsString = System.DateTime.Now.ToString();
                        loginfo = "Date: " + dateAsString +
                            ", Property: " + model.property +
                            ", Name: " + model.firstName + " " + model.lastName +
                            ", Reservation: " + model.bookingNumber +
                            ", ChartsPostPay status: " + pay.status.error.shorttext +
                            ", ChartsPostPay amount: " + prepay.paym.amnt +
                            ", AuthNetTrans transaction: " + result.GetValue(4).ToString() +
                            ", AuthNetTrans status: Voided" +
                        ";" + Environment.NewLine
                        + "-------------------" + Environment.NewLine;
                        logdata(loginfo);
                        #endregion
                    }
                }
                else
                {
                    ViewBag.Error = true;
                    ViewBag.Message = result.GetValue(2);
                    ViewBag.TransId = result.GetValue(4);

                    #region LogFile
                    dateAsString = System.DateTime.Now.ToString();
                    loginfo = "Date: " + dateAsString +
                        ", Property: " + model.property +
                        ", Name: " + model.firstName + " " + model.lastName +
                        ", Reservation: " + model.bookingNumber +
                        ", AuthNetTrans transaction: " + result.GetValue(4).ToString() +
                        ", AuthNetTrans status: " + result.GetValue(0).ToString() +
                        ", AuthNetTrans message: " + result.GetValue(2).ToString() +
                    ";" + Environment.NewLine
                    + "-------------------" + Environment.NewLine;
                    logdata(loginfo);
                    #endregion
                }
            }
            else
            {
                ViewBag.Error = true;
                ViewBag.Message = prepay.status.error.shorttext;
                ViewBag.TransId = 0;

                #region LogFile
                var dateAsString = System.DateTime.Now.ToString();
                string loginfo = "Date: " + dateAsString +
                    ", Property: " + model.property +
                    ", Name: " + model.firstName + " " + model.lastName +
                    ", Reservation: " + model.bookingNumber +
                    ", ChartsPrePost status: " + prepay.status.error.shorttext +
                ";" + Environment.NewLine
                + "-------------------" + Environment.NewLine;
                logdata(loginfo);
                #endregion
            }

            return View();
        }


        public string processCardNumbers(string cardnumbers)
        {
            string cardnums = cardnumbers.Replace(" ", "");
            int length = cardnums.Length;
            int tohide = length - 8;
            string aux = cardnums.Substring(0, 4);
            for (int i = 0; i < tohide; i++)
            {
                aux += "*";
            }
            aux += cardnums.Substring(length - 4, 4);
            
            return aux;
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