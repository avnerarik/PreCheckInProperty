using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingConfirm.Models
{
    #region Common
    public class propIdent
    {
        public string mesg { get; set; }
        public string propcode { get; set; }
        public login login { get; set; }
    }

    public class login
    {
        public string user { get; set; }
        public string pasw { set; get; }
    }

    public class Status
    {
        public DateTime timestamp;
        public double timeexec;
        public bool success;
        public Error error;
    }

    public class Error
    {
        public int code;
        public string shorttext;
    }

    public class rStatus
    {
        public bool success;
        public Error error;
        public string mesg;
        public string propcode;
        public string dllrevn;
    }
    #endregion

    #region ResvSummRequest
    public class jsonRequestModel
    {
        public Requests[] requests;
    }
    
    public class Requests
    {
        public propIdent ident = new propIdent();
        public string keyinfo { get; set; }
        public string reqn { get; set; }
        public string back { get; set; }
        public string watl { get; set; }
        public string name { get; set; }
        public string arrd { get; set; }
        public string comp { get; set; }
        public string lbkg { get; set; }
        public string site { get; set; }
        public string rbkg { get; set; }
        public string grup { get; set; }
        public string rtypc { get; set; }
        public string srch1 { get; set; }
        public string srch2 { get; set; }

    }
    #endregion;

    #region ResvFetchRequest
    public class jsonFetchModel
    {
        public Fetch[] requests;
    }

    public class Fetch
    {
        public propIdent ident { get; set; }
        public string indx { get; set; }
    }
    #endregion

    #region ResvFetch Response
    public class jsonFecthResponseModel
    {
        public Status status;
        public FetchResponse[] responses;
    }

    public class FetchResponse
    {
        public rStatus status;
        public int indx;
        public int lbkg;
        public int accn;
        public bool hold;
        public int holdsec;
        public string rpid;
        public Guest guest;
        public bool guestmemb;
        public Stay stay;
    }

    public class Guest
    {
        public string naml;
        public string namf;
        public string salu;
        public Addr addr;
        public string comp;
        public string rbnm;
        public string cont;
        public string phon;
        public string cell;
        public string emai;
        public string comm;
        public string secu;
    }

    public class Addr
    {
        public string line1;
        public string line2;
        public string city;
        public string posc;
        public string stat;
        public string cnty;
    }

    public class Stay
    {
        public string rbkg;
        public string sitc;
        public string arrd;
        public string arrt;
        public string depd;
        public string dept;
        public string bkgd;
        public string pkgc;
        public string grpc;
        public string srbc;
        public string mkac;
        public string rcod;
        public string allt;
        public string paym;
        public decimal totl;
        public decimal fcst;
        public Card card;
        public Exflds[] exflds;
        public RtypReq[] rtypreq;
    }

    public class RtypReq
    {
        public string rtypc;
        public int rtypq;
        public int adlt;
        public int chld;
        public RsvRoms[] rsvroms;
    }

    public class Exflds {
        public int numb { get; set; }
        public string value { get; set; }
    }

    public class RsvRoms
    {
        public string romc;
    }

    public class Card
    {
        public string numb;
        public string expy;
        public string auth;
    }

    #endregion
    
    #region ResvUpdate Request
    public class jsonUpdateModel
    {
        public Update[] requests;
    }
    
    public class Update
    {
        public propIdent ident = new propIdent();
        public string indx;
        public guest guest;
        public stay stay;
    }

    public class guest
    {
        public string naml;
        public string namf;
        public string salu;
        public addr addr;
        public string comp;
        public string comm;
        public string emai;
        public string phon;
        public string cell;
        public string secu;
    }

    public class stay {
        public string arrt { get; set; }
        public exflds[] exflds { get; set; }
        public string paym { get; set; }
        public Card card { get; set; }
    }

    public class exflds {
        public int numb { get; set; }
        public string value { get; set; }
    }

    public class addr
    {
        public string line1;
        public string line2;
        public string posc;
        public string city;
        public string stat;
        public string cnty;
    }
    
    public class jsonUpdateResponseModel
    {
        public Status status;
        public UpdateResponse[] responses;
    }

    public class UpdateResponse
    {
        public rStatus status;
    }
    #endregion

    #region ResvUpdateResponse
    public class jsonResponseModel
    {
        public Status status;
        public Responses[] responses;
    }

    public class Responses
    {
        public rStatus status;
        public Resvs[] resvs;
        public string keyinfo;
        public string more;
    }

    public class Resvs
    {
        public string watlf { get; set; }
        public string indx { get; set; }
        public string accn { get; set; }
        public string salu { get; set; }
        public string naml { get; set; }
        public string namf { get; set; }
        public string arrd { get; set; }
        public string depd { get; set; }
        public Rtyps[] rtyps { get; set; }
        public string lbkg { get; set; }
        public string site { get; set; }
        public string rbkg { get; set; }
        public string agen { get; set; }
        public string grup { get; set; }
        public string fnra { get; set; }
        public string cnty { get; set; }
        public string paym { get; set; }
        public string depa { get; set; }
        public string s1 { get; set; }
        public string s2 { get; set; }
    }

    public class Rtyps
    {
        public string rtypc { get; set; }
        public string rtypn { get; set; }
        public int rtypq { get; set; }
    }

    #endregion

    #region FieldList
    public class fieldListRequest
    {
        public fieldListFetch[] requests;
    }

    public class fieldListFetch
    {
        public propIdent ident = new propIdent();
        public string fldnam { get; set; }
    }

    public class fieldListResponse
    {
        public Status status;
        public fieldListResponseDetails[] responses;
    }

    public class fieldListResponseDetails {

        public rStatus status;
        public string[] fldvalues;
    }
    #endregion

    #region PaymentMethods
    public class paymFetchRequest
    {
        public paymFetch[] requests;
    }

    public class paymFetch
    {
        public propIdent ident = new propIdent();
        public int paytyp = 0;
        public string payc { get; set; }
    }

    public class paymFetchResponse
    {
        public Status status;
        public paymFetchResponseDetails[] responses;
    }

    public class paymFetchResponseDetails
    {
        public rStatus status;

        public Payms[] payms { get; set; }
    }

    public class Payms
    {
        public int indx { get; set; }

        public string payc { get; set; }

        public string name { get; set; }

        public string ctyp { get; set; }
    }
    #endregion

    #region TranObjPost
    public class TranObjPostRequest {
        public TranObjPost[] requests { get; set; }
    }

    public class TranObjPost {
        public propIdent ident { get; set; }

        public string obj { get; set; }

        public int indx { get; set; }

        public decimal amntnd { get; set; }

        public Trans tran { get; set; }
    }

    public class Trans {
        public string comt { get; set; }
        public string refr { get; set; }
        public string paym { get; set; }
        public string paytyp { get; set; }
        public decimal amnt { get; set; }
        public decimal comm { get; set; }
        public string rpid { get; set; }
    }

    public class TranObjPostResponse {
        public Status status;
        public TransPostResponses[] responses;
    }

    public class TransPostResponses {
        public rStatus status { get; set; }
        public string obj { get; set; }
        public int indx { get; set; }
        public int[] tranx { get; set; }
    }
    #endregion

    #region TranObjPrePost
    public class TranObjPrePostRequest {
        public TranObjPrePost[] requests { get; set; }
    }

    public class TranObjPrePost {
        public propIdent ident { get; set; }
        public string obj { get; set; }
        public int indx { get; set; }
        public decimal amntnd { get; set; }
        public Trans tran { get; set; }
    }

    public class TranObjPrePostResponse {
        public Status status;
        public TransPrePostResponses[] responses;
    }

    public class TransPrePostResponses {
        public rStatus status { get; set; }
        public Paym paym { get; set; }
    }

    public class Paym
    {
        public decimal amntnd { get; set; }
        public decimal change { get; set; }
        public decimal amnt { get; set; }
        public decimal round { get; set; }
        public decimal comm { get; set; }
    }
    #endregion
}

