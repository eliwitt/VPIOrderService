using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace VPIOrderService
{
    [ServiceContract]
    interface IOrderService
    {
        [FaultContractAttribute(typeof(SystemFault))]
        [FaultContractAttribute(typeof(DatabaseFault))]
        [OperationContract]
        [WebGet(UriTemplate = "/{companyCode}")]
        [Description("Returns a list of your orders for the last thirty days")]
        ICollection<CustOrder> GetOrders(string companyCode);

        [FaultContract(typeof(SystemFault))]
        [FaultContract(typeof(DatabaseFault))]
        [OperationContract]
        [WebGet(
            UriTemplate = "Order/{companyCode}/{ordernu}")]
        [Description("Returns the details of an order")]
        ICollection<CustOrder> GetOrder(string companyCode, string ordernu);
    }


    [DataContract]
    public class CustOrder
    {
        /// <summary>
        /// How manys days in the past to looks for orders
        /// </summary>
        [DataMember]
        public bool IsWebOrder { get; set; }
        [DataMember]
        public string ItemNumber { get; set; }
        [DataMember]
        public string CustomerCode { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public string LineNumber { get; set; }
        [DataMember]
        public string ShippingMethod { get; set; }
        [DataMember]
        public string TrackingNumber { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string CsrName { get; set; }
        [DataMember]
        public string CsrEmail { get; set; }
        [DataMember]
        public string Advertiser { get; set; }
        [DataMember]
        public string Design { get; set; }
        [DataMember]
        public DateTime TargetShipDate { get; set; }
        [DataMember]
        public DateTime ActualShipDate { get; set; }
        [DataMember]
        public DateTime TargetDeliveryDate { get; set; }
        [DataMember]
        public DateTime ActualDeliveryDate { get; set; }
        [DataMember]
        public string ReceivedBy { get; set; }
        [DataMember]
        public string PoNumber { get; set; }
        [DataMember]
        public string StatusText { get; set; }
    }
    // Classes for passing fault information back to client applications
    [DataContractAttribute]
    public class SystemFault
    {
        [DataMemberAttribute]
        public string SystemOperation { get; set; }

        [DataMemberAttribute]
        public string SystemReason { get; set; }

        [DataMemberAttribute]
        public string SystemMessage { get; set; }
    }

    [DataContractAttribute]
    public class DatabaseFault
    {
        [DataMemberAttribute]
        public string DbOperation { get; set; }

        [DataMemberAttribute]
        public string DbReason { get; set; }

        [DataMemberAttribute]
        public string DbMessage { get; set; }
    }
}
