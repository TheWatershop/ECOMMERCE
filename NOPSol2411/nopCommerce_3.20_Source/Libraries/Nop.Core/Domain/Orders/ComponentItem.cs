using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class ComponentItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Parent Item No identifier
        /// </summary>
        public string ParentItemId { get; set; }
        /// <summary>
        /// Gets or sets the ItemId identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product Name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the Expiry Date
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the Batch Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// Gets or sets the Parent Item Description
        /// </summary>
        public string ParentItemName { get; set; }

        /// Gets or sets the Parent Item Serial No.
        /// </summary>
        public string ParentItemSerial { get; set; }
        /// <summary>
        /// Gets the Location and Sublocation
        public string Location { get; set; }
        public string Sub_Location { get; set; }
        /// Gets the product
        /// </summary>
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
