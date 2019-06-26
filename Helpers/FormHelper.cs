using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using BookingConfirm.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace BookingConfirm.Helpers
{
    public static class FormHelper
    {
        private static readonly List<DocumentTypeModel> _doctype = new List<DocumentTypeModel> {
            new DocumentTypeModel() { value = "P", text = "Passport" },
            new DocumentTypeModel() { value = "I", text = "ID" }
        };

        private static readonly List<DocumentTypeModel> _genderlist = new List<DocumentTypeModel> {
            new DocumentTypeModel() { value = "M", text = "Male" },
            new DocumentTypeModel() { value = "F", text = "Female" }
        };

        private static readonly List<string> _arrvhours = new List<string> {
            "00:00","01:00","02:00","03:00","04:00","05:00","06:00","07:00","08:00","09:00","10:00","11:00",
            "12:00","13:00","14:00","15:00","16:00","17:00","18:00","19:00","20:00","21:00","22:00","23:00",
        };

        public static List<DocumentTypeModel> GetDocTypesList()
        {
            return _doctype; // return doctypes list
        }

        public static List<string> GetArrivalHours()
        {
            return _arrvhours; // return Hours list
        }

        public static List<DocumentTypeModel> GetGenderList()
        {
            return _genderlist; // return Hours list
        }


        public static List<DocumentTypeModel> RequestCountry(string propCode)
        {
            var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            JavaScriptSerializer js = new JavaScriptSerializer();

            login logindetails = new login();
            logindetails.user = "MGR";
            logindetails.pasw = "";

            propIdent property = new propIdent();
            property.mesg = "FieldListFetch";
            property.propcode = propCode;
            property.login = logindetails;

            fieldListFetch jsonfetch = new fieldListFetch();
            jsonfetch.ident = property;
            jsonfetch.fldnam = "Country";

            fieldListFetch[] sendarray = { jsonfetch };

            fieldListRequest array = new fieldListRequest();
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
            fieldListResponse fetchresponse = JsonConvert.DeserializeObject<fieldListResponse>(result);

            List<DocumentTypeModel> data = new List<DocumentTypeModel>();
            foreach (var item in fetchresponse.responses[0].fldvalues)
            {
                DocumentTypeModel temp = new DocumentTypeModel();
                temp.value = item;
                temp.text = item;
                data.Add(temp);
            }

            return data;
        }

        public static List<string> RequestSalutation(string propCode)
        {
            try
            {
                var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                JavaScriptSerializer js = new JavaScriptSerializer();

                login logindetails = new login();
                logindetails.user = "MGR";
                logindetails.pasw = "";

                propIdent property = new propIdent();
                property.mesg = "FieldListFetch";
                property.propcode = propCode;
                property.login = logindetails;

                fieldListFetch jsonfetch = new fieldListFetch();
                jsonfetch.ident = property;
                jsonfetch.fldnam = "salutation";

                fieldListFetch[] sendarray = { jsonfetch };

                fieldListRequest array = new fieldListRequest();
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
                fieldListResponse fetchresponse = JsonConvert.DeserializeObject<fieldListResponse>(result);

                List<string> data = new List<string>();
                foreach (var item in fetchresponse.responses[0].fldvalues)
                {
                    string temp = "";
                    temp = item;
                    data.Add(temp);

                    //DocumentTypeModel temp = new DocumentTypeModel();
                    //temp.value = item;
                    //temp.text = item;
                    //data.Add(temp);
                }

                return data;
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                List<string> data = new List<string>();
                return data;
            }
        }
        
        public static List<DocumentTypeModel> RequestPaymentMethod(string propCode, string paymc)
        {
            try
            {
                var webAddr = "https://chartswebintf-fra.chartspms.com.au/json/execute?un=charteuhh&pw=hh246eu";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                JavaScriptSerializer js = new JavaScriptSerializer();

                login logindetails = new login();
                logindetails.user = "MGR";
                logindetails.pasw = "";

                propIdent property = new propIdent();
                property.mesg = "PaymFetch";
                property.propcode = propCode;
                property.login = logindetails;

                paymFetch jsonfetch = new paymFetch();
                jsonfetch.ident = property;
                jsonfetch.payc = paymc;

                paymFetch[] sendarray = { jsonfetch };

                paymFetchRequest array = new paymFetchRequest();
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
                paymFetchResponse fetchresponse = JsonConvert.DeserializeObject<paymFetchResponse>(result);

                List<DocumentTypeModel> data = new List<DocumentTypeModel>();
                foreach (var item in fetchresponse.responses[0].payms)
                {
                    DocumentTypeModel temp = new DocumentTypeModel();
                    temp.value = item.payc;
                    temp.text = item.name;
                    data.Add(temp);
                }

                return data;
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                List<DocumentTypeModel> data = new List<DocumentTypeModel>();
                return data;
            }
        }
    }
}