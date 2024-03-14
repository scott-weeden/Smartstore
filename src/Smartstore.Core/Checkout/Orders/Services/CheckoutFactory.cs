﻿using Smartstore.Core.Checkout.Cart;
using Smartstore.Core.Checkout.Orders.Handlers;

namespace Smartstore.Core.Checkout.Orders
{
    public class CheckoutFactory : ICheckoutFactory
    {
        private readonly IEnumerable<Lazy<ICheckoutHandler, CheckoutHandlerMetadata>> _handlers;
        private readonly string _templateName;

        public CheckoutFactory(
            IEnumerable<Lazy<ICheckoutHandler, CheckoutHandlerMetadata>> handlers,
            ShoppingCartSettings shoppingCartSettings)
        {
            if (shoppingCartSettings.CheckoutTemplate.EqualsNoCase(CheckoutTemplateNames.Terminal))
            {
                _templateName = CheckoutTemplateNames.Terminal;

                _handlers = handlers
                    .Where(x => x.Metadata.HandlerType.Equals(typeof(ConfirmHandler)))
                    .OrderBy(x => x.Metadata.Order);
            }
            else
            {
                _handlers = handlers.OrderBy(x => x.Metadata.Order);
            }
        }

        public virtual CheckoutRequirements GetRequirements()
        {
            CheckoutRequirements requirements = 0;

            if (_handlers.Any(FindHandler(CheckoutActionNames.BillingAddress)))
            {
                requirements |= CheckoutRequirements.BillingAddress;
            }
            if (_handlers.Any(FindHandler(CheckoutActionNames.ShippingMethod)))
            {
                requirements |= CheckoutRequirements.Shipping;
            }
            if (_handlers.Any(FindHandler(CheckoutActionNames.PaymentMethod)))
            {
                requirements |= CheckoutRequirements.Payment;
            }

            return requirements;
        }

        public virtual CheckoutStep[] GetCheckoutSteps()
        {
            return _handlers.Select(Convert).ToArray();
        }

        public virtual CheckoutStep GetCheckoutStep(string action, string controller = "Checkout", string area = null)
        {
            Guard.NotEmpty(action);
            Guard.NotEmpty(controller);

            var handler = _handlers.FirstOrDefault(FindHandler(action, controller, area));

            return Convert(handler);
        }

        public virtual CheckoutStep GetNextCheckoutStep(CheckoutStep step, bool next)
        {
            Guard.NotNull(step);

            Lazy<ICheckoutHandler, CheckoutHandlerMetadata> handler;

            if (next)
            {
                handler = _handlers
                    .Where(x => x.Metadata.Order > step.Handler.Metadata.Order)
                    .OrderBy(x => x.Metadata.Order)
                    .FirstOrDefault();
            }
            else
            {
                handler = _handlers
                    .Where(x => x.Metadata.Order < step.Handler.Metadata.Order)
                    .OrderByDescending(x => x.Metadata.Order)
                    .FirstOrDefault();
            }

            return Convert(handler);
        }

        protected virtual CheckoutStep Convert(Lazy<ICheckoutHandler, CheckoutHandlerMetadata> handler)
        {
            if (handler == null)
            {
                return null;
            }

            var viewPath = _templateName == null ? null : $"{_templateName}.{handler.Metadata.DefaultAction}";

            return new(new(handler), viewPath);
        }

        private static Func<Lazy<ICheckoutHandler, CheckoutHandlerMetadata>, bool> FindHandler(string action, string controller = "Checkout", string area = null)
            => x => x.Metadata.Actions.Contains(action, StringComparer.OrdinalIgnoreCase)
                && x.Metadata.Controller.EqualsNoCase(controller)
                && x.Metadata.Area.NullEmpty().EqualsNoCase(area.NullEmpty());
    }
}