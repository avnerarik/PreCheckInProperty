using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using BookingConfirm.Models;

using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;

namespace BookingConfirm.Helpers
{
    public class ANetHelper
    {
        public static Array PostPay(string AuthorizeLoginID, string AuthorizeTransactionKey, FetchResponse bookingDetails, decimal AmountToBeCharged = 0)
        {
            Console.WriteLine("Charge Credit Card Sample");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = AuthorizeLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = AuthorizeTransactionKey,
            };

            

            var orderType = new orderType
            {
                invoiceNumber = bookingDetails.lbkg.ToString(),
                description = "PreCheckIn payment",
                
            };

            var customerData = new customerDataType
            {
                type = customerTypeEnum.individual,
                email = bookingDetails.guest.emai
            };

            var creditCard = new creditCardType
            {
                cardNumber = bookingDetails.stay.card.numb.Replace(" ",""),
                expirationDate = bookingDetails.stay.card.expy.Replace("/", ""),
                cardCode = bookingDetails.stay.card.auth
            };

            var billingAddress = new customerAddressType
            {
                firstName = bookingDetails.guest.namf,
                lastName = bookingDetails.guest.naml,
                email = bookingDetails.guest.emai,
                address = bookingDetails.guest.addr.line1,
                city = bookingDetails.guest.addr.city,
                state = bookingDetails.guest.addr.stat,
                country = bookingDetails.guest.addr.cnty,
                company = bookingDetails.guest.comp,
                phoneNumber = bookingDetails.guest.phon,
                zip = bookingDetails.guest.addr.posc
            };

            var retailType = new transRetailInfoType
            {
                marketType = "2",
                deviceType = "8"
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items
            var lineItems = new lineItemType[1];
            lineItems[0] = new lineItemType { itemId = "1", name = "PreCheckIn of booking " + bookingDetails.lbkg.ToString(), quantity = 1, unitPrice = bookingDetails.stay.totl, totalAmount = bookingDetails.stay.totl };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),// charge the card
                refTransId = bookingDetails.lbkg.ToString(),
                order = orderType,
                customer = customerData,
                lineItems = lineItems,
                payment = paymentType,
                amount = AmountToBeCharged,
                billTo = billingAddress,
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            Array response_array = new string[6];
            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        response_array.SetValue(response.transactionResponse.responseCode, 0); //Response Code
                        response_array.SetValue(response.transactionResponse.messages[0].code, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.messages[0].description, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.lbkg.ToString(), 5); //Reference ID


                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        //Failed Transaction.
                        if (response.transactionResponse.errors != null)
                        {
                            response_array.SetValue("2", 0); //Response Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                            response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                            response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                            response_array.SetValue(bookingDetails.lbkg.ToString(), 5); //Reference ID

                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    //Failed Transaction.
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        response_array.SetValue("2", 0); //Response Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.lbkg.ToString(), 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        response_array.SetValue("3", 0); //Response Code
                        response_array.SetValue(response.messages.message[0].code, 1); //Message Code
                        response_array.SetValue(response.messages.message[0].text, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.lbkg.ToString(), 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
                response_array.SetValue("-1", 0);
                response_array.SetValue(bookingDetails.lbkg.ToString(), 5); //Reference ID
            }

            //return response;
            return response_array;
        }

        public static Array Void(string AuthorizeLoginID, string AuthorizeTransactionKey, FetchResponse bookingDetails, string TransactionID)
        {
            Console.WriteLine("Void Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = AuthorizeLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = AuthorizeTransactionKey,
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),    // refund type
                refTransId = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            Array response_array = new string[6];
            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        response_array.SetValue(response.transactionResponse.responseCode, 0); //Response Code
                        response_array.SetValue(response.transactionResponse.messages[0].code, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.messages[0].description, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID


                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            response_array.SetValue("2", 0); //Response Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                            response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                            response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                            response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        response_array.SetValue("2", 0); //Response Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        response_array.SetValue("3", 0); //Response Code
                        response_array.SetValue(response.messages.message[0].code, 1); //Message Code
                        response_array.SetValue(response.messages.message[0].text, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
                response_array.SetValue("-1", 0);
                response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID
            }

            //return response;
            return response_array;
        }

        public static Array Refund(string AuthorizeLoginID, string AuthorizeTransactionKey, FetchResponse bookingDetails, decimal AmountToBeCharged, string TransactionID)
        {
            Console.WriteLine("Refund Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = AuthorizeLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = AuthorizeTransactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = bookingDetails.stay.card.numb,
                expirationDate = bookingDetails.stay.card.expy.Replace("/", ""),
                cardCode = bookingDetails.stay.card.auth
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                payment = paymentType,
                amount = AmountToBeCharged,
                refTransId = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            Array response_array = new string[6];
            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        response_array.SetValue(response.transactionResponse.responseCode, 0); //Response Code
                        response_array.SetValue(response.transactionResponse.messages[0].code, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.messages[0].description, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            response_array.SetValue(2, 0); //Response Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                            response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                            response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                            response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                            response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        response_array.SetValue(2, 0); //Response Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorCode, 1); //Message Code
                        response_array.SetValue(response.transactionResponse.errors[0].errorText, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        response_array.SetValue(3, 0); //Response Code
                        response_array.SetValue(response.messages.message[0].code, 1); //Message Code
                        response_array.SetValue(response.messages.message[0].text, 2); //Description
                        response_array.SetValue(response.transactionResponse.authCode, 3); //Success, Auth Code
                        response_array.SetValue(response.transactionResponse.transId, 4); //Transaction ID
                        response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID

                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
                response_array.SetValue(-1, 0);
                response_array.SetValue(bookingDetails.stay.rbkg, 5); //Reference ID
            }

            return response_array;
        }
    }
}