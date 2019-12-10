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
        public bool IsEditCustomer { get; set; }
        public bool IsEditItemInventory { get; set; }
        public bool IsEditInvoce { get; set; }
        public List<IQbCustomer> ListCustomer { get; set; }
        public List<IQbItemInventory> ListItemInventory { get; set; }
        public List<IQbInvoce> ListInvoce { get; set; }
        public List<string> ID = new List<string>();
    }
    public class IHubData
    {
        public string data { get; set; }
    }

    public class IQbModel
    { 
        public string ListID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
    public class IQbCustomer:IQbModel
    {
    }
    public class IQbItemInventory : IQbModel
    {
        
    }
    public class IQbInvoce
    {
        public string TxnID { get; set; }
        public DateTime TxnDate { get; set; }//DATE
        public DateTime DueDate { get; set; }//DUE DATE
        public int RefNumber { get; set; }//NUM
        public bool IsPaid { get; set; }
        public decimal Subtotal { get; set; }//AMOUNT
        public decimal BalanceRemaining { get; set; }//OPEN BALANCE

    }
}
namespace qbService.Models.Customer
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class QBXML
    {

        private QBXMLQBXMLMsgsRs qBXMLMsgsRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRs QBXMLMsgsRs
        {
            get
            {
                return this.qBXMLMsgsRsField;
            }
            set
            {
                this.qBXMLMsgsRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRs
    {

        private QBXMLQBXMLMsgsRsCustomerQueryRs customerQueryRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRs CustomerQueryRs
        {
            get
            {
                return this.customerQueryRsField;
            }
            set
            {
                this.customerQueryRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRs
    {

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRet[] customerRetField;

        private byte requestIDField;

        private byte statusCodeField;

        private string statusSeverityField;

        private string statusMessageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CustomerRet")]
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRet[] CustomerRet
        {
            get
            {
                return this.customerRetField;
            }
            set
            {
                this.customerRetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte requestID
        {
            get
            {
                return this.requestIDField;
            }
            set
            {
                this.requestIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte statusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusSeverity
        {
            get
            {
                return this.statusSeverityField;
            }
            set
            {
                this.statusSeverityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusMessage
        {
            get
            {
                return this.statusMessageField;
            }
            set
            {
                this.statusMessageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRet
    {

        private string listIDField;

        private System.DateTime timeCreatedField;

        private System.DateTime timeModifiedField;

        private uint editSequenceField;

        private string nameField;

        private string fullNameField;

        private bool isActiveField;

        private byte sublevelField;

        private uint phoneField;

        private bool phoneFieldSpecified;

        private decimal balanceField;

        private decimal totalBalanceField;

        private string jobStatusField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeCreated
        {
            get
            {
                return this.timeCreatedField;
            }
            set
            {
                this.timeCreatedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeModified
        {
            get
            {
                return this.timeModifiedField;
            }
            set
            {
                this.timeModifiedField = value;
            }
        }

        /// <remarks/>
        public uint EditSequence
        {
            get
            {
                return this.editSequenceField;
            }
            set
            {
                this.editSequenceField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }

        /// <remarks/>
        public bool IsActive
        {
            get
            {
                return this.isActiveField;
            }
            set
            {
                this.isActiveField = value;
            }
        }

        /// <remarks/>
        public byte Sublevel
        {
            get
            {
                return this.sublevelField;
            }
            set
            {
                this.sublevelField = value;
            }
        }

        /// <remarks/>
        public uint Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PhoneSpecified
        {
            get
            {
                return this.phoneFieldSpecified;
            }
            set
            {
                this.phoneFieldSpecified = value;
            }
        }

        /// <remarks/>
        public decimal Balance
        {
            get
            {
                return this.balanceField;
            }
            set
            {
                this.balanceField = value;
            }
        }

        /// <remarks/>
        public decimal TotalBalance
        {
            get
            {
                return this.totalBalanceField;
            }
            set
            {
                this.totalBalanceField = value;
            }
        }

        /// <remarks/>
        public string JobStatus
        {
            get
            {
                return this.jobStatusField;
            }
            set
            {
                this.jobStatusField = value;
            }
        }
    }


}
namespace qbService.Models.ItemInventory 
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class QBXML
    {

        private QBXMLQBXMLMsgsRs qBXMLMsgsRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRs QBXMLMsgsRs
        {
            get
            {
                return this.qBXMLMsgsRsField;
            }
            set
            {
                this.qBXMLMsgsRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRs
    {

        private QBXMLQBXMLMsgsRsItemInventoryQueryRs itemInventoryQueryRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRsItemInventoryQueryRs ItemInventoryQueryRs
        {
            get
            {
                return this.itemInventoryQueryRsField;
            }
            set
            {
                this.itemInventoryQueryRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRs
    {

        private QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRet[] itemInventoryRetField;

        private ushort requestIDField;

        private byte statusCodeField;

        private string statusSeverityField;

        private string statusMessageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemInventoryRet")]
        public QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRet[] ItemInventoryRet
        {
            get
            {
                return this.itemInventoryRetField;
            }
            set
            {
                this.itemInventoryRetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort requestID
        {
            get
            {
                return this.requestIDField;
            }
            set
            {
                this.requestIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte statusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusSeverity
        {
            get
            {
                return this.statusSeverityField;
            }
            set
            {
                this.statusSeverityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusMessage
        {
            get
            {
                return this.statusMessageField;
            }
            set
            {
                this.statusMessageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRet
    {

        private string listIDField;

        private System.DateTime timeCreatedField;

        private System.DateTime timeModifiedField;

        private uint editSequenceField;

        private string nameField;

        private string fullNameField;

        private bool isActiveField;

        private byte sublevelField;

        private string manufacturerPartNumberField;

        private decimal salesPriceField;

        private QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetIncomeAccountRef incomeAccountRefField;

        private decimal purchaseCostField;

        private QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetCOGSAccountRef cOGSAccountRefField;

        private QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetPrefVendorRef prefVendorRefField;

        private QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetAssetAccountRef assetAccountRefField;

        private sbyte quantityOnHandField;

        private decimal averageCostField;

        private byte quantityOnOrderField;

        private byte quantityOnSalesOrderField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeCreated
        {
            get
            {
                return this.timeCreatedField;
            }
            set
            {
                this.timeCreatedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeModified
        {
            get
            {
                return this.timeModifiedField;
            }
            set
            {
                this.timeModifiedField = value;
            }
        }

        /// <remarks/>
        public uint EditSequence
        {
            get
            {
                return this.editSequenceField;
            }
            set
            {
                this.editSequenceField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }

        /// <remarks/>
        public bool IsActive
        {
            get
            {
                return this.isActiveField;
            }
            set
            {
                this.isActiveField = value;
            }
        }

        /// <remarks/>
        public byte Sublevel
        {
            get
            {
                return this.sublevelField;
            }
            set
            {
                this.sublevelField = value;
            }
        }

        /// <remarks/>
        public string ManufacturerPartNumber
        {
            get
            {
                return this.manufacturerPartNumberField;
            }
            set
            {
                this.manufacturerPartNumberField = value;
            }
        }

        /// <remarks/>
        public decimal SalesPrice
        {
            get
            {
                return this.salesPriceField;
            }
            set
            {
                this.salesPriceField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetIncomeAccountRef IncomeAccountRef
        {
            get
            {
                return this.incomeAccountRefField;
            }
            set
            {
                this.incomeAccountRefField = value;
            }
        }

        /// <remarks/>
        public decimal PurchaseCost
        {
            get
            {
                return this.purchaseCostField;
            }
            set
            {
                this.purchaseCostField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetCOGSAccountRef COGSAccountRef
        {
            get
            {
                return this.cOGSAccountRefField;
            }
            set
            {
                this.cOGSAccountRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetPrefVendorRef PrefVendorRef
        {
            get
            {
                return this.prefVendorRefField;
            }
            set
            {
                this.prefVendorRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetAssetAccountRef AssetAccountRef
        {
            get
            {
                return this.assetAccountRefField;
            }
            set
            {
                this.assetAccountRefField = value;
            }
        }

        /// <remarks/>
        public sbyte QuantityOnHand
        {
            get
            {
                return this.quantityOnHandField;
            }
            set
            {
                this.quantityOnHandField = value;
            }
        }

        /// <remarks/>
        public decimal AverageCost
        {
            get
            {
                return this.averageCostField;
            }
            set
            {
                this.averageCostField = value;
            }
        }

        /// <remarks/>
        public byte QuantityOnOrder
        {
            get
            {
                return this.quantityOnOrderField;
            }
            set
            {
                this.quantityOnOrderField = value;
            }
        }

        /// <remarks/>
        public byte QuantityOnSalesOrder
        {
            get
            {
                return this.quantityOnSalesOrderField;
            }
            set
            {
                this.quantityOnSalesOrderField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetIncomeAccountRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetCOGSAccountRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetPrefVendorRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsItemInventoryQueryRsItemInventoryRetAssetAccountRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }



}
namespace qbService.Models.Invoce {


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class QBXML
    {

        private QBXMLQBXMLMsgsRs qBXMLMsgsRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRs QBXMLMsgsRs
        {
            get
            {
                return this.qBXMLMsgsRsField;
            }
            set
            {
                this.qBXMLMsgsRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRs
    {

        private QBXMLQBXMLMsgsRsInvoiceQueryRs invoiceQueryRsField;

        /// <remarks/>
        public QBXMLQBXMLMsgsRsInvoiceQueryRs InvoiceQueryRs
        {
            get
            {
                return this.invoiceQueryRsField;
            }
            set
            {
                this.invoiceQueryRsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsInvoiceQueryRs
    {

        private QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRet[] invoiceRetField;

        private byte statusCodeField;

        private string statusSeverityField;

        private string statusMessageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("InvoiceRet")]
        public QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRet[] InvoiceRet
        {
            get
            {
                return this.invoiceRetField;
            }
            set
            {
                this.invoiceRetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte statusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusSeverity
        {
            get
            {
                return this.statusSeverityField;
            }
            set
            {
                this.statusSeverityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string statusMessage
        {
            get
            {
                return this.statusMessageField;
            }
            set
            {
                this.statusMessageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRet
    {

        private string txnIDField;

        private System.DateTime timeCreatedField;

        private System.DateTime timeModifiedField;

        private uint editSequenceField;

        private byte txnNumberField;

        private QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetCustomerRef customerRefField;

        private QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetARAccountRef aRAccountRefField;

        private QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetTemplateRef templateRefField;

        private System.DateTime txnDateField;

        private byte refNumberField;

        private bool isPendingField;

        private bool isFinanceChargeField;

        private System.DateTime dueDateField;

        private System.DateTime shipDateField;

        private decimal subtotalField;

        private decimal salesTaxPercentageField;

        private decimal salesTaxTotalField;

        private decimal appliedAmountField;

        private decimal balanceRemainingField;

        private bool isPaidField;

        private bool isToBePrintedField;

        private bool isToBeEmailedField;

        /// <remarks/>
        public string TxnID
        {
            get
            {
                return this.txnIDField;
            }
            set
            {
                this.txnIDField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeCreated
        {
            get
            {
                return this.timeCreatedField;
            }
            set
            {
                this.timeCreatedField = value;
            }
        }

        /// <remarks/>
        public System.DateTime TimeModified
        {
            get
            {
                return this.timeModifiedField;
            }
            set
            {
                this.timeModifiedField = value;
            }
        }

        /// <remarks/>
        public uint EditSequence
        {
            get
            {
                return this.editSequenceField;
            }
            set
            {
                this.editSequenceField = value;
            }
        }

        /// <remarks/>
        public byte TxnNumber
        {
            get
            {
                return this.txnNumberField;
            }
            set
            {
                this.txnNumberField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetCustomerRef CustomerRef
        {
            get
            {
                return this.customerRefField;
            }
            set
            {
                this.customerRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetARAccountRef ARAccountRef
        {
            get
            {
                return this.aRAccountRefField;
            }
            set
            {
                this.aRAccountRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetTemplateRef TemplateRef
        {
            get
            {
                return this.templateRefField;
            }
            set
            {
                this.templateRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime TxnDate
        {
            get
            {
                return this.txnDateField;
            }
            set
            {
                this.txnDateField = value;
            }
        }

        /// <remarks/>
        public byte RefNumber
        {
            get
            {
                return this.refNumberField;
            }
            set
            {
                this.refNumberField = value;
            }
        }

        /// <remarks/>
        public bool IsPending
        {
            get
            {
                return this.isPendingField;
            }
            set
            {
                this.isPendingField = value;
            }
        }

        /// <remarks/>
        public bool IsFinanceCharge
        {
            get
            {
                return this.isFinanceChargeField;
            }
            set
            {
                this.isFinanceChargeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DueDate
        {
            get
            {
                return this.dueDateField;
            }
            set
            {
                this.dueDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ShipDate
        {
            get
            {
                return this.shipDateField;
            }
            set
            {
                this.shipDateField = value;
            }
        }

        /// <remarks/>
        public decimal Subtotal
        {
            get
            {
                return this.subtotalField;
            }
            set
            {
                this.subtotalField = value;
            }
        }

        /// <remarks/>
        public decimal SalesTaxPercentage
        {
            get
            {
                return this.salesTaxPercentageField;
            }
            set
            {
                this.salesTaxPercentageField = value;
            }
        }

        /// <remarks/>
        public decimal SalesTaxTotal
        {
            get
            {
                return this.salesTaxTotalField;
            }
            set
            {
                this.salesTaxTotalField = value;
            }
        }

        /// <remarks/>
        public decimal AppliedAmount
        {
            get
            {
                return this.appliedAmountField;
            }
            set
            {
                this.appliedAmountField = value;
            }
        }

        /// <remarks/>
        public decimal BalanceRemaining
        {
            get
            {
                return this.balanceRemainingField;
            }
            set
            {
                this.balanceRemainingField = value;
            }
        }

        /// <remarks/>
        public bool IsPaid
        {
            get
            {
                return this.isPaidField;
            }
            set
            {
                this.isPaidField = value;
            }
        }

        /// <remarks/>
        public bool IsToBePrinted
        {
            get
            {
                return this.isToBePrintedField;
            }
            set
            {
                this.isToBePrintedField = value;
            }
        }

        /// <remarks/>
        public bool IsToBeEmailed
        {
            get
            {
                return this.isToBeEmailedField;
            }
            set
            {
                this.isToBeEmailedField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetCustomerRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetARAccountRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsInvoiceQueryRsInvoiceRetTemplateRef
    {

        private string listIDField;

        private string fullNameField;

        /// <remarks/>
        public string ListID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        /// <remarks/>
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }
    }




}
