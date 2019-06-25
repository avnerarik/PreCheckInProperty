using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using BookingConfirm.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace BookingConfirm.Helpers
{
    public class JsonRequestsHelper
    {
        public static FetchResponse RequestBookingDetails(string propCode, string index)
        {
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
            property.propcode = propCode;
            property.login = logdetails;

            Fetch jsonfetch = new Fetch();
            jsonfetch.ident = property;
            jsonfetch.indx = index;

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

            FetchResponse data = fetchresponse.responses[0];
            
            return data;

        }

        public static UpdateResponse UpdateBookingDetails(string propCode, BookingViewModel model)
        {
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
            requestdata.propcode = propCode;
            requestdata.login = logdetails;

            Update jsonupdate = new Update();

            guest g = new guest();
            g.salu = model.salutation;
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
            
            UpdateResponse data = response.responses[0];

            return data;
        }
    }
}