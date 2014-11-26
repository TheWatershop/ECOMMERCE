using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Components
{
    public partial class ComponentItemMap : EntityTypeConfiguration<ComponentItem>
    {
        public ComponentItemMap()
        {
            this.ToTable("ComponentItem");
            this.HasKey(ComponentItem => ComponentItem.Id);
            this.Property(ComponentItem => ComponentItem.ParentItemId);
            //this.Property(ComponentItem => ComponentItem.CustomerId);
            //this.Property(ComponentItem => ComponentItem.ProductId);
            this.Property(ComponentItem => ComponentItem.ExpiryDate);
            this.Property(ComponentItem => ComponentItem.Quantity);
            this.Property(ComponentItem => ComponentItem.ParentItemName);
            this.Property(ComponentItem => ComponentItem.ParentItemSerial);
            this.Property(ComponentItem => ComponentItem.Location);
            this.Property(ComponentItem => ComponentItem.Sub_Location);


            this.HasRequired(ComponentItem => ComponentItem.Customer)
                .WithMany()
                //.WithMany(o => o.ComponentItems)
                .HasForeignKey(ComponentItem => ComponentItem.CustomerId);

            this.HasRequired(ComponentItem => ComponentItem.Product)
                .WithMany()
                .HasForeignKey(ComponentItem => ComponentItem.ProductId);
        }
    }
}