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
            g.salu = model.salu;
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
            s.paym = model.paym;
            if (model.cardNumber != null)
            {
                s.card = new Card();
                s.card.auth = model.cardHolder;
                s.card.numb = model.cardNumber;
                s.card.expy = model.cardEndDate;
            }

            List<exflds> ext = new List<exflds>();

            if (model.gender != null)
            {
                exflds e = new exflds();
                e.numb = 65;
                e.value = model.gender;
                ext.Add(e);
            }

            //if (ext.Count > 0)
            //{
            //    s.exflds = ext.ToArray();
            //}

            //if (false) {
            //List<exflds> ext = new List<exflds>();

            if (model.document != null)
            {
                exflds e = new exflds();
                e.numb = 74;
                e.value = model.document;
                ext.Add(e);
            }

            if (model.idtype != null)
            {
                exflds n = new exflds();
                n.numb = 75;
                n.value = model.idtype;
                ext.Add(n);
            }

            if (model.secu != null)
            {
                exflds n = new exflds();
                n.numb = 76;
                n.value = model.secu;
                ext.Add(n);
            }

            if (ext.Count > 0)
            {
                s.exflds = ext.ToArray();
            }
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

        public static TransPrePostResponses PrePostTransactionDetails(string propCode, FetchResponse model)
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
            property.mesg = "TranObjPrePost";
            property.propcode = propCode;
            property.login = logdetails;

            Trans tran = new Trans();
            tran.paytyp = "A";
            tran.paym = model.stay.paym;
            tran.comt = "PrePost AuthNet advance payment";
            tran.amnt = model.stay.fcst;
            tran.comm = 0;
            tran.rpid = "MGR";

            TranObjPrePost jsonfetch = new TranObjPrePost();
            jsonfetch.ident = property;
            jsonfetch.indx = model.indx;
            jsonfetch.obj = "RESV";
            jsonfetch.amntnd = model.stay.fcst;
            jsonfetch.tran = tran;

            TranObjPrePost[] sendarray = { jsonfetch };

            TranObjPrePostRequest array = new TranObjPrePostRequest();
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
            TranObjPrePostResponse fetchresponse = JsonConvert.DeserializeObject<TranObjPrePostResponse>(result);

            TransPrePostResponses data = fetchresponse.responses[0];

            return data;
        }

        public static TransPostResponses PostTransactionDetails(string propCode, FetchResponse bookingDetails ,TransPrePostResponses model, Array aunet)
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
            property.mesg = "TranObjPost";
            property.propcode = propCode;
            property.login = logdetails;

            Trans tran = new Trans();
            tran.paytyp = "A";
            tran.paym = bookingDetails.stay.paym;
            tran.comt = "Payment:" + aunet.GetValue(4).ToString();
            tran.amnt = model.paym.amnt;
            tran.comm = 0;// model.paym.comm;
            tran.rpid = "MGR";

            TranObjPost jsonfetch = new TranObjPost();
            jsonfetch.ident = property;
            jsonfetch.indx = bookingDetails.indx;
            jsonfetch.obj = "RESV";
            jsonfetch.amntnd = model.paym.amnt;
            jsonfetch.tran = tran;
            
            TranObjPost[] sendarray = { jsonfetch };

            TranObjPostRequest array = new TranObjPostRequest();
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
            TranObjPostResponse fetchresponse = JsonConvert.DeserializeObject<TranObjPostResponse>(result);

            TransPostResponses data = fetchresponse.responses[0];

            return data;
        }
    }
}