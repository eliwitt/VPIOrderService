using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using IBM.Data.DB2.iSeries;
using Vincent.Util;
using System.Web;
using System.Web.Hosting;
using System.Web.Configuration;

namespace VPIOrderService
{
    /// <summary>
    /// Process requests for order information.
    /// </summary>
    public class OrderService : IOrderService
    {
        private static iDB2DataReader dr;
        private static Dictionary<String, String> queries = new Dictionary<string,string>();
        private static string customer;
        private static string ornu;
        /// <summary>
        /// Retrieve the given order for the customer
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public ICollection<CustOrder> GetOrders(string companyCode)
        {
            LogIt("In GetOrders " + companyCode);
            customer = companyCode;
            ornu = "";
            string start = DateTime.Now.AddDays(-30).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            string end = DateTime.Now.AddDays(10).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            string sql = GetQuery("CustOrders").Replace("{CUST_NO}", companyCode).Replace("{FDATE}", start).Replace("{TDATE}", end);
            List<CustOrder> data = RetrieveOrders(sql);
            return data;
        }
        /// <summary>
        /// Retrieve the given order for the customer
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ICollection<CustOrder> GetOrder(string companyCode, string ordernu)
        {
            LogIt("In GetOrder " + companyCode + " " + ordernu);
            customer = companyCode;
            ornu = ordernu;
            // build the query string
            string queryStr = GetQuery("RetrieveOrder").Replace("{CUST_NO}", companyCode).Replace("{ORDER_NO}", ordernu);
            List<CustOrder> data = RetrieveOrders(queryStr);
            return data;
        }
        /// <summary>
        /// Using the sql param retrieve the given customer orders
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static List<CustOrder> RetrieveOrders(string sql)
        {
            LogIt("In RetrieveOrders " + sql);
            List<CustOrder> data = new List<CustOrder>();
            try
            {
                using (iDB2Connection conn = Config.Conn)
                {

                    dr = ExecuteSql(sql, conn);
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CustOrder tmp = RowToDict(dr);
                            if (tmp != null)
                            {
                                data.Add(tmp);
                            }
                        }
                    }
                    else
                    {
                        data.Add(new CustOrder() { CustomerCode = customer, OrderNumber = ornu, StatusText = "Order(s) not found" });
                    }

                }
            }
            catch (iDB2CommErrorException ex)
            {
                throw new FaultException<DatabaseFault>(new DatabaseFault()
                {
                    DbOperation = "Connection to database",
                    DbReason = "Exception accessing database",
                    DbMessage = ex.Message
                }, "Database connection issue");
            }
            return data;
        }
        /// <summary>
        /// Using the data reader build the classes internal collection.
        /// May have to change this routine if I can not get the serializer
        /// to work correctly.
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static CustOrder RowToDict(iDB2DataReader dr)
        {
            CustOrder data = new CustOrder();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                //row.Add(dr.GetName(i).Trim(), dr.GetValue(i).ToString().Trim());
                //  we can not use the above techinque we have to load a basic object
                //
                switch(dr.GetName(i).Trim())
                {
                    case "ITEMNO":
                        data.ItemNumber = dr.GetValue(i).ToString().Trim();
                        break;
                    case "CUSTNO":
                        data.CustomerCode = dr.GetValue(i).ToString().Trim();
                        break;
                    case "CUSTNAM":
                        data.CustomerName = dr.GetValue(i).ToString().Trim();
                        break;
                    case "ORDNO":
                        data.OrderNumber = dr.GetValue(i).ToString().Trim();
                        break;
                    case "ORLINE":
                        data.LineNumber = dr.GetValue(i).ToString().Trim();
                        break;
                    case "SHPMTH":
                        data.ShippingMethod = dr.GetValue(i).ToString().Trim();
                        break;
                    case "TRACK":
                        data.TrackingNumber = dr.GetValue(i).ToString().Trim();
                        break;
                    case "STATUS":
                        data.Status = dr.GetValue(i).ToString().Trim();
                        break;
                    case "CSRNAM":
                        data.CsrName = dr.GetValue(i).ToString().Trim();
                        break;
                    case "CSREML":
                        data.CsrEmail = dr.GetValue(i).ToString().Trim();
                        break;
                    case "ADVTSR":
                        data.Advertiser = dr.GetValue(i).ToString().Trim();
                        break;
                    case "DESIGN":
                        data.Design = dr.GetValue(i).ToString().Trim();
                        break;
                    case "RCVDBY":
                        data.ReceivedBy = dr.GetValue(i).ToString().Trim();
                        break;
                    case "PONUM":
                        data.PoNumber = dr.GetValue(i).ToString().Trim();
                        break;
                    case "TGTSDT":
                        if (dr.GetValue(i).ToString().Trim() != "")
                            if (dr.GetValue(i).ToString().Trim() == "0" || dr.GetValue(i).ToString().Trim() == "99999999")
                            { }
                            else { data.TargetShipDate = StringUtil.ParseIbsDate(dr.GetValue(i).ToString().Trim()); }
                        break;
                    case "ACTSDT":
                        if (dr.GetValue(i).ToString().Trim() != "")
                            if (dr.GetValue(i).ToString().Trim() == "0" || dr.GetValue(i).ToString().Trim() == "99999999")
                            { }
                            else { data.ActualShipDate = StringUtil.ParseIbsDate(dr.GetValue(i).ToString().Trim()); }
                        break;
                    case "TGTDLD":
                        if (dr.GetValue(i).ToString().Trim() != "")
                            if (dr.GetValue(i).ToString().Trim() == "0" || dr.GetValue(i).ToString().Trim() == "99999999")
                            { }
                            else { data.TargetDeliveryDate = StringUtil.ParseIbsDate(dr.GetValue(i).ToString().Trim()); }
                        break;
                    case "ACTDLD":
                        if (dr.GetValue(i).ToString().Trim() != "")
                            if (dr.GetValue(i).ToString().Trim() == "0" || dr.GetValue(i).ToString().Trim() == "99999999")
                            { }
                            else { data.ActualDeliveryDate = StringUtil.ParseIbsDate(dr.GetValue(i).ToString().Trim()); }
                        break;
                }
            }
            switch (data.Status)
            {
                case "00500":
                    data.StatusText = "Not Confirmed";
                    break;
                case "01000":
                    data.StatusText = "Please upload art";
                    break;
                case "02000":
                    data.StatusText = "Order Confirmation";
                    break;
                case "03000":
                    data.StatusText = "Proof Ready for Approval";
                    break;
                case "04000":
                    data.StatusText = "Proof Approved";
                    break;
                case "05000":
                    data.StatusText = "In Production - Ripped";
                    break;
                case "06000":
                    data.StatusText = "In Production - Printed";
                    break;
                case "07000":
                    data.StatusText = "In Production - Finished";
                    break;
                case "08000":
                    data.StatusText = "Shipped";
                    break;
                case "09000":
                    data.StatusText = "Delivered";
                    break;
                case "10000":
                    data.StatusText = "Order Invoiced";
                    break;
                default:
                    data.StatusText = "ERROR";
                    break;
            }
            return data;
        }
        /// <summary>
        /// With the sql string and the active connection retrieve the data
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static iDB2DataReader ExecuteSql(string sql, iDB2Connection conn)
        {
          
            iDB2DataReader d_rdr = null;
            try
            {
                using (iDB2Command cmd = new iDB2Command(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    d_rdr = cmd.ExecuteReader();
                }
            }
            catch (iDB2DCFunctionErrorException ex)
            {
                throw new FaultException<DatabaseFault>(new DatabaseFault()
                {
                    DbOperation = "Reading the database",
                    DbReason = "Exception accessing database",
                    DbMessage = ex.InnerException.Message
                }, "Reading db error");
            }
            catch (iDB2SQLErrorException ex)
            {
                throw new FaultException<DatabaseFault>(new DatabaseFault()
                {
                    DbOperation = "Sql error",
                    DbReason = "Exception accessing database",
                    DbMessage = ex.Message
                }, "Sql error");
            }
            return d_rdr;
        }
        /// <summary>
        /// Retrieve the sql text to be used to retrieve the data.
        /// </summary>
        /// <param name="theQuery"></param>
        /// <returns></returns>
        private static String GetQuery(string theQuery)
        {
            LogIt(HostingEnvironment.MapPath("~/Queries/" + theQuery + ".sql"));

            if (!queries.ContainsKey(theQuery)) {
                using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Queries/" + theQuery + ".sql")))
                {
                    queries.Add(theQuery, sr.ReadToEnd());
                }
            }
            return queries[theQuery];
        }
        /// <summary>
        /// Logs text in an effort to debug
        /// </summary>
        /// <param name="logWhat"></param>
        private static void LogIt(string logWhat)
        {
            string logFile = HostingEnvironment.MapPath("~/temp/whatlog.txt");

            File.AppendAllText(logFile, DateTime.Now.ToString() + "\n");
            File.AppendAllText(logFile, "What: " + logWhat + "\n");
            File.AppendAllText(logFile, "--------------------------------------------\n");
        }
    }
}