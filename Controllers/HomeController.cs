using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingConfirm.Models;
using BookingConfirm.Helpers;
using Newtonsoft.Json;

namespace BookingConfirm.Controllers
{
    public class HomeController : BaseController
    {

        private IImagesRepository _repository;
        public HomeController(): this(new ImagesRepository())
        {
        }

        public HomeController(IImagesRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Index(string prop = "")
        {
            NameValueCollection nvc = Request.QueryString;

            //SetCulture("en");
            string pCode = prop != "" ? prop : nvc["pCode"];
            string bNumber = nvc["bNumber"];
            string lName = nvc["lName"];
            ViewBag.pCode = System.Configuration.ConfigurationManager.AppSettings["propcode"];//"smart";// pCode;
            ViewBag.bNumber = bNumber;
            ViewBag.lName = lName;

            ViewBag.pict = "";

            if (TempData["Model"] != null)
            {
                ClientViewModel model = TempData["Model"] as ClientViewModel;
                ViewBag.lName = model.lname;
                ViewBag.bNumber = model.reservNumber;
                ViewBag.pCode = model.propCode;
                string response = "Invalid entry attempt, try again.";
                ModelState.AddModelError("", response);

                return View(model);
            }

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(ClientViewModel model)
        {
            if (!ModelState.IsValid || model.propCode == null)
            {
                ViewBag.fName = model.fname;
                ViewBag.lName = model.lname;
                ViewBag.bNumber = model.reservNumber;
                ViewBag.pCode = model.propCode;
                if (model.propCode == null)
                {
                    ModelState.AddModelError("", "Property code missing.");
                }
                ModelState.AddModelError("", "Information not valid.");

                //ViewBag.pict = _repository.getImageByProp(model.propCode).url;
                return View(model);
            }
            else
            {
                string fname = model.fname;
                string lname = model.lname;
                string resvn = model.reservNumber;
                string pcode = model.propCode;

                #region Json Request
                var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                JavaScriptSerializer js = new JavaScriptSerializer();

                login logdetails = new login();
                logdetails.user = "MGR";
                logdetails.pasw = "";

                propIdent property = new propIdent();
                property.mesg = "ResvSumm";
                property.propcode = pcode;
                property.login = logdetails;

                Requests jsonrequest = new Requests();
                jsonrequest.ident = property;
                jsonrequest.reqn = "10";
                jsonrequest.name = lname;
                jsonrequest.lbkg = resvn;

                Requests[] sendarray = { jsonrequest };

                jsonRequestModel array = new jsonRequestModel();
                array.requests = sendarray;

                string json = JsonConvert.SerializeObject(array);


                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                #endregion

                string result = streamReader.ReadToEnd(); //in result is the end data.

                jsonResponseModel response = JsonConvert.DeserializeObject<jsonResponseModel>(result);

                //if success
                if (response.responses[0].status.success)
                {
                    if (response.responses[0].resvs.Length > 0)
                    {
                        var data = response.responses[0].resvs[0];
                        if (data.cnty.ToUpper() == "FRANCE")
                        {
                            SetCulture("fr");
                        }
                        else
                        {
                            SetCulture("en");
                        }

                        TempData["jsonResponse"] = response;
                        return RedirectToAction("BookingData", "Client", new { pcode = pcode, indx = response.responses[0].resvs[0].indx });
                    }
                    else
                    {
                        ViewBag.pCode = model.propCode;
                        ModelState.AddModelError("", "No valid reservation found.");
                        return View(model);
                    }
                }
            }

            ViewBag.pCode = model.propCode;
            ModelState.AddModelError("", "Invalid entry attempt.");
            return View(model);
        }

        public void SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddHours(1);
            }
            Response.Cookies.Add(cookie);
            return;//RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}