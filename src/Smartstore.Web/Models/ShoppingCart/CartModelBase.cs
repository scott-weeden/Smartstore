﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Smartstore.Core.Catalog.Products;
using Smartstore.Core.Localization;
using Smartstore.Web.Models.Catalog;
using Smartstore.Web.Models.Media;

namespace Smartstore.Web.Models.Cart
{
    public abstract class CartModelBase : ModelBase
    {
        public abstract IEnumerable<CartEntityModelBase> Items { get; }

        public bool ShowSku { get; set; }
        public bool ShowProductImages { get; set; }
        public bool ShowProductBundleImages { get; set; }
        public bool IsEditable { get; set; }
        public int BundleThumbSize { get; set; }
        public bool DisplayShortDesc { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    public abstract class CartEntityModelBase : EntityModelBase, IQuantityInput
    {
        public string Sku { get; set; }
        public ImageModel Image { get; set; } = new();

        public int ProductId { get; set; }
        public LocalizedValue<string> ProductName { get; set; }
        public string ProductSeName { get; set; }
        public string ProductUrl { get; set; }
        public ProductType ProductType { get; set; }
        public bool VisibleIndividually { get; set; }

        public Money UnitPrice { get; set; }
        public Money SubTotal { get; set; }
        public Money Discount { get; set; }
        public string BasePrice { get; set; }
        public CartItemPriceModel Price { get; set; } = new();

        public int EnteredQuantity { get; set; }
        public LocalizedValue<string> QuantityUnitName { get; set; }
        public List<SelectListItem> AllowedQuantities { get; set; } = new();
        public int MinOrderAmount { get; set; }
        public int MaxOrderAmount { get; set; }
        public int QuantityStep { get; set; }
        public QuantityControlType QuantityControlType { get; set; }

        public string AttributeInfo { get; set; }
        public string RecurringInfo { get; set; }
        public List<string> Warnings { get; set; } = new();
        public LocalizedValue<string> ShortDesc { get; set; }

        public bool BundlePerItemPricing { get; set; }
        public bool BundlePerItemShoppingCart { get; set; }
        public BundleItemModel BundleItem { get; set; } = new();

        public abstract IEnumerable<CartEntityModelBase> ChildItems { get; }
        public DateTime CreatedOnUtc { get; set; }
    }

    public partial class BundleItemModel : EntityModelBase
    {
        public string PriceWithDiscount { get; set; }
        public int DisplayOrder { get; set; }
        public bool HideThumbnail { get; set; }
    }

    public partial class CartItemPriceModel : PriceModel
    {
        /// <summary>
        /// Unit price actually is <see cref="PriceModel.FinalPrice" />
        /// </summary>
        public Money UnitPrice 
        {
            get => FinalPrice;
        }

        public int Quantity { get; set; } = 1;

        public Money SubTotal { get; set; }
        public bool ShowSavingBadge { get; set; }
        public bool ShowPriceLabel { get; set; }
        public LocalizedString CountdownText { get; set; }
    }
}