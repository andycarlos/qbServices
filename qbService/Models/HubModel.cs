using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qbService.Models
{
    public static class HubUser
    {
        public static List<IHubUser> HubUsers = new List<IHubUser>();
    }
    public class IHubUser
    {
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public string ConectionId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> Token = new List<string>();
        public Boolean StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<IQbCustomer> ListCustomer { get; set; }
        public List<IQbItem> ListItems { get; set; }
        public List<IQbInvoce> ListInvoce { get; set; }
        public List<string> ID = new List<string>();
    }
    public class IHubData
    {
        public string Token { get; set; }
        public string Body { get; set; }
    }

    public class IQbModel
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
    public class IQbCustomer : IQbModel
    {
        public decimal CreditLimit { get; set; }
    }
    public class IQbItem : IQbModel
    {
        public string SalesDesc { get; set; }
        public double SalesPrice { get; set; }
        public string Type { get; set; }
    }
    public class IQbInvoce
    {
        public string TxnID { get; set; }
        public DateTime TxnDate { get; set; }//DATE
        public DateTime DueDate { get; set; }//DUE DATE
        public string RefNumber { get; set; }//NUM
        public bool IsPaid { get; set; }
        public decimal Subtotal { get; set; }//AMOUNT
        public decimal BalanceRemaining { get; set; }//OPEN BALANCE

    }
    public class IQbInvoceFilter
    {
        public string CustomerID { get; set; }
        public string PaidStatus { get; set; }
        public Boolean Overdue { get; set; }
    }
    public class IQbSaleOrder{
        public string CustomerRefListID { get; set; }
        public List<IQbSalesOrderLineAdd> SalesOrderLineAdd { get; set; }
    }
    public class IQbSalesOrderLineAdd
    {
        public string ItemRefListID { get; set; }
        public double Quantity { get; set; }
    }
}
namespace qbService.Models.Item
{

    public class Rootobject
    {
        public Xml xml { get; set; }
        public QBXML QBXML { get; set; }
    }

    public class Xml
    {
        public string version { get; set; }
    }

    public class QBXML
    {
        public Qbxmlmsgsrs QBXMLMsgsRs { get; set; }
    }

    public class Qbxmlmsgsrs
    {
        public Itemqueryrs ItemQueryRs { get; set; }
    }

    public class Itemqueryrs
    {
        public string statusCode { get; set; }
        public string statusSeverity { get; set; }
        public string statusMessage { get; set; }
        public Itemserviceret[] ItemServiceRet { get; set; }
        public Itemnoninventoryret ItemNonInventoryRet { get; set; }
        public Itemotherchargeret[] ItemOtherChargeRet { get; set; }
        public Iteminventoryret[] ItemInventoryRet { get; set; }
        public Itemfixedassetret[] ItemFixedAssetRet { get; set; }
        public Itemdiscountret ItemDiscountRet { get; set; }
    }

    public class Itemnoninventoryret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Sublevel { get; set; }
        public Salesorpurchase SalesOrPurchase { get; set; }
    }

    public class Salesorpurchase
    {
        public string Desc { get; set; }
        public string Price { get; set; }
        public Accountref AccountRef { get; set; }
    }

    public class Accountref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Itemdiscountret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Sublevel { get; set; }
        public string ItemDesc { get; set; }
        public Salestaxcoderef SalesTaxCodeRef { get; set; }
        public string DiscountRatePercent { get; set; }
        public Accountref1 AccountRef { get; set; }
    }

    public class Salestaxcoderef
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Accountref1
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Itemserviceret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Sublevel { get; set; }
        public Salesorpurchase1 SalesOrPurchase { get; set; }
    }

    public class Salesorpurchase1
    {
        public string Desc { get; set; }
        public string Price { get; set; }
        public Accountref2 AccountRef { get; set; }
    }

    public class Accountref2
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Itemotherchargeret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Sublevel { get; set; }
        public Salesorpurchase2 SalesOrPurchase { get; set; }
        public Salestaxcoderef1 SalesTaxCodeRef { get; set; }
    }

    public class Salesorpurchase2
    {
        public string Desc { get; set; }
        public string Price { get; set; }
        public Accountref3 AccountRef { get; set; }
    }

    public class Accountref3
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Salestaxcoderef1
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Iteminventoryret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string IsActive { get; set; }
        public string Sublevel { get; set; }
        public string SalesDesc { get; set; }
        public string SalesPrice { get; set; }
        public Incomeaccountref IncomeAccountRef { get; set; }
        public string PurchaseDesc { get; set; }
        public Cogsaccountref COGSAccountRef { get; set; }
        public Assetaccountref AssetAccountRef { get; set; }
        public string QuantityOnHand { get; set; }
        public string AverageCost { get; set; }
        public string QuantityOnOrder { get; set; }
        public string QuantityOnSalesOrder { get; set; }
        public Parentref ParentRef { get; set; }
        public string PurchaseCost { get; set; }
        public Unitofmeasuresetref UnitOfMeasureSetRef { get; set; }
        public Prefvendorref PrefVendorRef { get; set; }
        public string ReorderPoint { get; set; }
        public string Max { get; set; }
        public string ManufacturerPartNumber { get; set; }
    }

    public class Incomeaccountref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Cogsaccountref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Assetaccountref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Parentref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Unitofmeasuresetref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Prefvendorref
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

    public class Itemfixedassetret
    {
        public string ListID { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public string EditSequence { get; set; }
        public string Name { get; set; }
        public string IsActive { get; set; }
        public string AcquiredAs { get; set; }
        public string PurchaseDesc { get; set; }
        public string PurchaseDate { get; set; }
        public string PurchaseCost { get; set; }
        public string VendorOrPayeeName { get; set; }
        public Assetaccountref1 AssetAccountRef { get; set; }
    }

    public class Assetaccountref1
    {
        public string ListID { get; set; }
        public string FullName { get; set; }
    }

}

