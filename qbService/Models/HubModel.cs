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
        public List<IQbVendors> ListVendors { get; set; }
        public List<IQbEmployee> ListEmployee { get; set; }
        public List<IQbOtherName> ListOtherName { get; set; }
        public List<IQbSalesRep> ListSalesRep { get; set; }
        //public List<string> ID = new List<string>();
    }
    public class IHubData
    {
        public string Token { get; set; }
        public string Body { get; set; }
        public string Funcion { get; set; }
    }

    public class IQbModel
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
    public class IQbCustomer : IQbModel
    {
        public decimal? CreditLimit { get; set; }
        public string Email { get; set; }
        public string SaleRepListID { get; set; }
        public List<ShipToAddress>? ShipToAddress { get; set; }
    }
    public class ShipToAddress
    {

        private string nameField;

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string noteField;

        private string defaultShipToField;

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
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }

        /// <remarks/>
        public string DefaultShipTo
        {
            get
            {
                return this.defaultShipToField;
            }
            set
            {
                this.defaultShipToField = value;
            }
        }
    }

    public class IQbVendors
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class IQbEmployee
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class IQbOtherName
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
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
        public List<IQbInvoiceLineItem> Items { get; set; }
    }
    public class IQbInvoceFilter
    {
        public string CustomerID { get; set; }
        public string PaidStatus { get; set; }
        public Boolean Overdue { get; set; }
        public Boolean includeLineItems { get; set; }
    }
    public class IQbInvoiceLineItem
    {
        public string ListID { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
    }
    public class IQbSaleOrder
    {
        public string CustomerRefListID { get; set; }
        public List<IQbSalesOrderLineAdd> SalesOrderLineAdd { get; set; }
    }
    public class IQbSalesOrderLineAdd
    {
        public string ItemRefListID { get; set; }
        public double Quantity { get; set; }
        public decimal Amount { get; set; }
    }
    public class IQbSalesRep
    {
        public string SaleRepListID { get; set; }
        public string UserListID { get; set; }
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

        private string statusCodeField;

        private string statusSeverityField;

        private string statusMessageField;

        private string retCountField;

        private string iteratorRemainingCountField;

        private string iteratorIDField;

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
        public string statusCode
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string retCount
        {
            get
            {
                return this.retCountField;
            }
            set
            {
                this.retCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string iteratorRemainingCount
        {
            get
            {
                return this.iteratorRemainingCountField;
            }
            set
            {
                this.iteratorRemainingCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string iteratorID
        {
            get
            {
                return this.iteratorIDField;
            }
            set
            {
                this.iteratorIDField = value;
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

        private string timeCreatedField;

        private string timeModifiedField;

        private string editSequenceField;

        private string nameField;

        private string fullNameField;

        private string isActiveField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetClassRef classRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetParentRef parentRefField;

        private string sublevelField;

        private string companyNameField;

        private string salutationField;

        private string firstNameField;

        private string middleNameField;

        private string lastNameField;

        private string jobTitleField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddress billAddressField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddressBlock billAddressBlockField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddress shipAddressField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddressBlock shipAddressBlockField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipToAddress[] shipToAddressField;

        private string phoneField;

        private string altPhoneField;

        private string faxField;

        private string emailField;

        private string ccField;

        private string contactField;

        private string altContactField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalContactRef additionalContactRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRet[] contactsRetField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCustomerTypeRef customerTypeRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetTermsRef termsRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesRepRef salesRepRefField;

        private string balanceField;

        private string totalBalanceField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesTaxCodeRef salesTaxCodeRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetItemSalesTaxRef itemSalesTaxRefField;

        private string salesTaxCountryField;

        private string resaleNumberField;

        private string accountNumberField;

        private string creditLimitField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPreferredPaymentMethodRef preferredPaymentMethodRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCreditCardInfo creditCardInfoField;

        private string jobStatusField;

        private string jobStartDateField;

        private string jobProjectedEndDateField;

        private string jobEndDateField;

        private string jobDescField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetJobTypeRef jobTypeRefField;

        private string notesField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalNotesRet[] additionalNotesRetField;

        private string preferredDeliveryMethodField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPriceLevelRef priceLevelRefField;

        private string externalGUIDField;

        private string taxRegistrationNumberField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCurrencyRef currencyRefField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetDataExtRet[] dataExtRetField;

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
        public string TimeCreated
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
        public string TimeModified
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
        public string EditSequence
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
        public string IsActive
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
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetClassRef ClassRef
        {
            get
            {
                return this.classRefField;
            }
            set
            {
                this.classRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetParentRef ParentRef
        {
            get
            {
                return this.parentRefField;
            }
            set
            {
                this.parentRefField = value;
            }
        }

        /// <remarks/>
        public string Sublevel
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
        public string CompanyName
        {
            get
            {
                return this.companyNameField;
            }
            set
            {
                this.companyNameField = value;
            }
        }

        /// <remarks/>
        public string Salutation
        {
            get
            {
                return this.salutationField;
            }
            set
            {
                this.salutationField = value;
            }
        }

        /// <remarks/>
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        public string MiddleName
        {
            get
            {
                return this.middleNameField;
            }
            set
            {
                this.middleNameField = value;
            }
        }

        /// <remarks/>
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        public string JobTitle
        {
            get
            {
                return this.jobTitleField;
            }
            set
            {
                this.jobTitleField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddress BillAddress
        {
            get
            {
                return this.billAddressField;
            }
            set
            {
                this.billAddressField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddressBlock BillAddressBlock
        {
            get
            {
                return this.billAddressBlockField;
            }
            set
            {
                this.billAddressBlockField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddress ShipAddress
        {
            get
            {
                return this.shipAddressField;
            }
            set
            {
                this.shipAddressField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddressBlock ShipAddressBlock
        {
            get
            {
                return this.shipAddressBlockField;
            }
            set
            {
                this.shipAddressBlockField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ShipToAddress")]
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipToAddress[] ShipToAddress
        {
            get
            {
                return this.shipToAddressField;
            }
            set
            {
                this.shipToAddressField = value;
            }
        }

        /// <remarks/>
        public string Phone
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
        public string AltPhone
        {
            get
            {
                return this.altPhoneField;
            }
            set
            {
                this.altPhoneField = value;
            }
        }

        /// <remarks/>
        public string Fax
        {
            get
            {
                return this.faxField;
            }
            set
            {
                this.faxField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string Cc
        {
            get
            {
                return this.ccField;
            }
            set
            {
                this.ccField = value;
            }
        }

        /// <remarks/>
        public string Contact
        {
            get
            {
                return this.contactField;
            }
            set
            {
                this.contactField = value;
            }
        }

        /// <remarks/>
        public string AltContact
        {
            get
            {
                return this.altContactField;
            }
            set
            {
                this.altContactField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalContactRef AdditionalContactRef
        {
            get
            {
                return this.additionalContactRefField;
            }
            set
            {
                this.additionalContactRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContactsRet")]
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRet[] ContactsRet
        {
            get
            {
                return this.contactsRetField;
            }
            set
            {
                this.contactsRetField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCustomerTypeRef CustomerTypeRef
        {
            get
            {
                return this.customerTypeRefField;
            }
            set
            {
                this.customerTypeRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetTermsRef TermsRef
        {
            get
            {
                return this.termsRefField;
            }
            set
            {
                this.termsRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesRepRef SalesRepRef
        {
            get
            {
                return this.salesRepRefField;
            }
            set
            {
                this.salesRepRefField = value;
            }
        }

        /// <remarks/>
        public string Balance
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
        public string TotalBalance
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
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesTaxCodeRef SalesTaxCodeRef
        {
            get
            {
                return this.salesTaxCodeRefField;
            }
            set
            {
                this.salesTaxCodeRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetItemSalesTaxRef ItemSalesTaxRef
        {
            get
            {
                return this.itemSalesTaxRefField;
            }
            set
            {
                this.itemSalesTaxRefField = value;
            }
        }

        /// <remarks/>
        public string SalesTaxCountry
        {
            get
            {
                return this.salesTaxCountryField;
            }
            set
            {
                this.salesTaxCountryField = value;
            }
        }

        /// <remarks/>
        public string ResaleNumber
        {
            get
            {
                return this.resaleNumberField;
            }
            set
            {
                this.resaleNumberField = value;
            }
        }

        /// <remarks/>
        public string AccountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        /// <remarks/>
        public string CreditLimit
        {
            get
            {
                return this.creditLimitField;
            }
            set
            {
                this.creditLimitField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPreferredPaymentMethodRef PreferredPaymentMethodRef
        {
            get
            {
                return this.preferredPaymentMethodRefField;
            }
            set
            {
                this.preferredPaymentMethodRefField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCreditCardInfo CreditCardInfo
        {
            get
            {
                return this.creditCardInfoField;
            }
            set
            {
                this.creditCardInfoField = value;
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

        /// <remarks/>
        public string JobStartDate
        {
            get
            {
                return this.jobStartDateField;
            }
            set
            {
                this.jobStartDateField = value;
            }
        }

        /// <remarks/>
        public string JobProjectedEndDate
        {
            get
            {
                return this.jobProjectedEndDateField;
            }
            set
            {
                this.jobProjectedEndDateField = value;
            }
        }

        /// <remarks/>
        public string JobEndDate
        {
            get
            {
                return this.jobEndDateField;
            }
            set
            {
                this.jobEndDateField = value;
            }
        }

        /// <remarks/>
        public string JobDesc
        {
            get
            {
                return this.jobDescField;
            }
            set
            {
                this.jobDescField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetJobTypeRef JobTypeRef
        {
            get
            {
                return this.jobTypeRefField;
            }
            set
            {
                this.jobTypeRefField = value;
            }
        }

        /// <remarks/>
        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AdditionalNotesRet")]
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalNotesRet[] AdditionalNotesRet
        {
            get
            {
                return this.additionalNotesRetField;
            }
            set
            {
                this.additionalNotesRetField = value;
            }
        }

        /// <remarks/>
        public string PreferredDeliveryMethod
        {
            get
            {
                return this.preferredDeliveryMethodField;
            }
            set
            {
                this.preferredDeliveryMethodField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPriceLevelRef PriceLevelRef
        {
            get
            {
                return this.priceLevelRefField;
            }
            set
            {
                this.priceLevelRefField = value;
            }
        }

        /// <remarks/>
        public string ExternalGUID
        {
            get
            {
                return this.externalGUIDField;
            }
            set
            {
                this.externalGUIDField = value;
            }
        }

        /// <remarks/>
        public string TaxRegistrationNumber
        {
            get
            {
                return this.taxRegistrationNumberField;
            }
            set
            {
                this.taxRegistrationNumberField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCurrencyRef CurrencyRef
        {
            get
            {
                return this.currencyRefField;
            }
            set
            {
                this.currencyRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DataExtRet")]
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetDataExtRet[] DataExtRet
        {
            get
            {
                return this.dataExtRetField;
            }
            set
            {
                this.dataExtRetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetClassRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetParentRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddress
    {

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string noteField;

        /// <remarks/>
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetBillAddressBlock
    {

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        /// <remarks/>
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddress
    {

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string noteField;

        /// <remarks/>
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipAddressBlock
    {

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        /// <remarks/>
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetShipToAddress
    {

        private string nameField;

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string noteField;

        private string defaultShipToField;

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
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }

        /// <remarks/>
        public string DefaultShipTo
        {
            get
            {
                return this.defaultShipToField;
            }
            set
            {
                this.defaultShipToField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalContactRef
    {

        private string contactNameField;

        private string contactValueField;

        /// <remarks/>
        public string ContactName
        {
            get
            {
                return this.contactNameField;
            }
            set
            {
                this.contactNameField = value;
            }
        }

        /// <remarks/>
        public string ContactValue
        {
            get
            {
                return this.contactValueField;
            }
            set
            {
                this.contactValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRet
    {

        private string listIDField;

        private string timeCreatedField;

        private string timeModifiedField;

        private string editSequenceField;

        private string contactField;

        private string salutationField;

        private string firstNameField;

        private string middleNameField;

        private string lastNameField;

        private string jobTitleField;

        private QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRetAdditionalContactRef additionalContactRefField;

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
        public string TimeCreated
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
        public string TimeModified
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
        public string EditSequence
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
        public string Contact
        {
            get
            {
                return this.contactField;
            }
            set
            {
                this.contactField = value;
            }
        }

        /// <remarks/>
        public string Salutation
        {
            get
            {
                return this.salutationField;
            }
            set
            {
                this.salutationField = value;
            }
        }

        /// <remarks/>
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        public string MiddleName
        {
            get
            {
                return this.middleNameField;
            }
            set
            {
                this.middleNameField = value;
            }
        }

        /// <remarks/>
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        public string JobTitle
        {
            get
            {
                return this.jobTitleField;
            }
            set
            {
                this.jobTitleField = value;
            }
        }

        /// <remarks/>
        public QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRetAdditionalContactRef AdditionalContactRef
        {
            get
            {
                return this.additionalContactRefField;
            }
            set
            {
                this.additionalContactRefField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetContactsRetAdditionalContactRef
    {

        private string contactNameField;

        private string contactValueField;

        /// <remarks/>
        public string ContactName
        {
            get
            {
                return this.contactNameField;
            }
            set
            {
                this.contactNameField = value;
            }
        }

        /// <remarks/>
        public string ContactValue
        {
            get
            {
                return this.contactValueField;
            }
            set
            {
                this.contactValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCustomerTypeRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetTermsRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesRepRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetSalesTaxCodeRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetItemSalesTaxRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPreferredPaymentMethodRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCreditCardInfo
    {

        private string creditCardNumberField;

        private string expirationMonthField;

        private string expirationYearField;

        private string nameOnCardField;

        private string creditCardAddressField;

        private string creditCardPostalCodeField;

        /// <remarks/>
        public string CreditCardNumber
        {
            get
            {
                return this.creditCardNumberField;
            }
            set
            {
                this.creditCardNumberField = value;
            }
        }

        /// <remarks/>
        public string ExpirationMonth
        {
            get
            {
                return this.expirationMonthField;
            }
            set
            {
                this.expirationMonthField = value;
            }
        }

        /// <remarks/>
        public string ExpirationYear
        {
            get
            {
                return this.expirationYearField;
            }
            set
            {
                this.expirationYearField = value;
            }
        }

        /// <remarks/>
        public string NameOnCard
        {
            get
            {
                return this.nameOnCardField;
            }
            set
            {
                this.nameOnCardField = value;
            }
        }

        /// <remarks/>
        public string CreditCardAddress
        {
            get
            {
                return this.creditCardAddressField;
            }
            set
            {
                this.creditCardAddressField = value;
            }
        }

        /// <remarks/>
        public string CreditCardPostalCode
        {
            get
            {
                return this.creditCardPostalCodeField;
            }
            set
            {
                this.creditCardPostalCodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetJobTypeRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetAdditionalNotesRet
    {

        private string noteIDField;

        private string dateField;

        private string noteField;

        /// <remarks/>
        public string NoteID
        {
            get
            {
                return this.noteIDField;
            }
            set
            {
                this.noteIDField = value;
            }
        }

        /// <remarks/>
        public string Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetPriceLevelRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetCurrencyRef
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
    public partial class QBXMLQBXMLMsgsRsCustomerQueryRsCustomerRetDataExtRet
    {

        private string ownerIDField;

        private string dataExtNameField;

        private string dataExtTypeField;

        private string dataExtValueField;

        /// <remarks/>
        public string OwnerID
        {
            get
            {
                return this.ownerIDField;
            }
            set
            {
                this.ownerIDField = value;
            }
        }

        /// <remarks/>
        public string DataExtName
        {
            get
            {
                return this.dataExtNameField;
            }
            set
            {
                this.dataExtNameField = value;
            }
        }

        /// <remarks/>
        public string DataExtType
        {
            get
            {
                return this.dataExtTypeField;
            }
            set
            {
                this.dataExtTypeField = value;
            }
        }

        /// <remarks/>
        public string DataExtValue
        {
            get
            {
                return this.dataExtValueField;
            }
            set
            {
                this.dataExtValueField = value;
            }
        }
    }





}

