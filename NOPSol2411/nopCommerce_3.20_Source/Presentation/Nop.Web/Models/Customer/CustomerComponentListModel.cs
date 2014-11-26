using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerComponentListModel : BaseNopModel
    {
        public CustomerComponentListModel()
        {
            Components = new List<ComponentDetailsModel>();
        }

        public IList<ComponentDetailsModel> Components { get; set; }

        public CustomerNavigationModel NavigationModel { get; set; }


        #region Nested classes
        public partial class ComponentDetailsModel : BaseNopEntityModel
        {
            public int Id { get; set; }
            public string ParentItemId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public DateTime ExpiryDate { get; set; }
            public int Quantity { get; set; }
            public string ParentItemName { get; set; }
            public string ParentItemSerial { get; set; }
            public string Location { get; set; }
            public string Sub_Location { get; set; }
        }
        #endregion
    }
}