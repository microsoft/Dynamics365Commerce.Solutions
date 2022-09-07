/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { Module, Node } from "@msdyn365-commerce-modules/utilities";
import { PriceComponent } from "@msdyn365-commerce/components";
import {
  CartLine,
  ChannelDeliveryOptionConfiguration,
  ChargeLine,
} from "@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g";
import * as React from "react";
import {
  ICheckoutViewProps,
  ILineItem,
  ILineItemDeliveryGroup,
  ILineItems,
  IPickUpAtStore,
} from "@msdyn365-commerce-modules/checkout";
import { IEmailDelivery } from "@msdyn365-commerce-modules/checkout";
import { IInvoicePaymentSummary } from "@msdyn365-commerce-modules/checkout/src/modules/checkout/components/get-invoice-payment-summary";

export const PickUpAtStoreComponent: React.FC<IPickUpAtStore> = ({
  PickUpAtStore,
  label,
  location,
}) => (
  <Node {...PickUpAtStore}>
    {label}
    {location}
  </Node>
);

export const EmailDeliveryComponent: React.FC<IEmailDelivery> = ({
  EmailDelivery,
  label,
}) => <Node {...EmailDelivery}>{label}</Node>;

export const LineItemComponent: React.FC<ILineItem> = ({
  LineItem,
  item,
  pickUpAtStore,
  emailDelivery,
}) => (
  <Node {...LineItem}>
    {item}
    {pickUpAtStore && <PickUpAtStoreComponent {...pickUpAtStore} />}
    {emailDelivery && <EmailDeliveryComponent {...emailDelivery} />}
  </Node>
);

export const LineItemGroupComponent: React.FC<ILineItemDeliveryGroup> = ({
  LineItemDeliveryGroup,
  LineItemList,
  heading,
  lineItems,
}) => (
  <Node {...LineItemDeliveryGroup}>
    {heading}
    <Node {...LineItemList}>
      {lineItems.map((lineItem) => (
        <LineItemComponent key={lineItem.LineId} {...lineItem} />
      ))}
    </Node>
  </Node>
);

export const LineItemGroupComponentWithMultiplePickUp: React.FC<ILineItemDeliveryGroup> = ({
  LineItemDeliveryGroup,
  LineItemList,
  heading,
  lineItems,
  lineItemWraper,
  lineItemWraperIcon,
}) => (
  <Node {...LineItemDeliveryGroup}>
    {lineItemWraperIcon}
    {lineItemWraper}
    {heading}
    <Node {...LineItemList}>
      {lineItems.map((lineItem) => (
        <LineItemComponentWithMultiplePickUp
          key={lineItem.LineId}
          {...lineItem}
        />
      ))}
    </Node>
  </Node>
);

export const LineItemComponentWithMultiplePickUp: React.FC<ILineItem> = ({
  LineItem,
  item,
  pickUpAtStore,
  emailDelivery,
}) => (
  <Node {...LineItem}>
    {item}
    {emailDelivery && <EmailDeliveryComponent {...emailDelivery} />}
  </Node>
);

export const PickUpAtStoreComponentWithMultiplePickUp: React.FC<IPickUpAtStore> = ({
  PickUpAtStore,
  label,
  location,
}) => (
  <Node {...PickUpAtStore}>
    {label}
    {location}
  </Node>
);

export const LineItemsComponent: React.FC<ILineItems> = ({
  LineItems,
  Header,
  heading,
  editLink,
  itemsForPickup,
  itemsForShip,
  itemsForEmail,
  itemsGroupWithMulitplePickupMode,
}) => (
  <Node {...LineItems}>
    <Node {...Header}>
      {heading}
      {editLink}
    </Node>
    {itemsGroupWithMulitplePickupMode === undefined && itemsForPickup && (
      <LineItemGroupComponent {...itemsForPickup} />
    )}
    {itemsGroupWithMulitplePickupMode === undefined && itemsForEmail && (
      <LineItemGroupComponent {...itemsForEmail} />
    )}
    {itemsGroupWithMulitplePickupMode === undefined && itemsForShip && (
      <LineItemGroupComponent {...itemsForShip} />
    )}
    {itemsGroupWithMulitplePickupMode !== undefined
      ? itemsGroupWithMulitplePickupMode.map((item, index) => {
          return (
            <LineItemGroupComponentWithMultiplePickUp {...item} key={index} />
          );
        })
      : null}
  </Node>
);
function _renderSubTotal(props: ICheckoutViewProps): JSX.Element | null {
  const OrderSubTotal =
    props.data.checkout.result?.checkoutCart.cart.SubtotalSalesAmount;
  return (
    <p className={`msc-order-summary__line-sub-total`}>
      {OrderSubTotal && OrderSubTotal > 0 ? (
        <>
          <span className="msc-order-summary__label">
            {props.resources.subTotalLabel}
          </span>
          <PriceComponent
            data={{
              price: { CustomerContextualPrice: OrderSubTotal },
            }}
            freePriceText={undefined}
            context={props.context}
            id={props.id}
            typeName={props.typeName}
            className={"msc-order-summary__value"}
          />
        </>
      ) : null}
    </p>
  );
}
const getDeliveryMode = (
  deliveryMode?: string,
  featureSate: boolean = false,
  channelDeliveryOptionConfig?: ChannelDeliveryOptionConfiguration,
  pickupDeliveryMode?: string
) => {
  if (!featureSate) {
    return pickupDeliveryMode;
  }
  return channelDeliveryOptionConfig?.PickupDeliveryModeCodes?.find(
    (dm: string | undefined) => dm === deliveryMode
  );
};

function _renderShipping(props: ICheckoutViewProps): JSX.Element | null {
  const cartlines =
    props.data.checkout.result?.checkoutCart.cart.CartLines || [];
  const headerChargelines =
    props.data.checkout.result?.checkoutCart.cart.ChargeLines || [];
  let freightFee = 0;
  let isShippingCalculated = false;
  //Calculating shipping from chargelines at header level
  if (headerChargelines && headerChargelines.length > 0) {
    const shippingChargeLines = (headerChargelines || []).filter(
      (chargeLine) => chargeLine.IsShipping
    );
    shippingChargeLines &&
      shippingChargeLines.map((Chargeline: ChargeLine) => {
        const chargeLine = Chargeline.NetAmountWithAllInclusiveTax || 0;
        isShippingCalculated = true;
        freightFee = freightFee + chargeLine;
      });
  } else {
    //Calculating shipping from chargelines at cart line level
    cartlines &&
      cartlines.map((cartline: CartLine) => {
        const shippingChargeLines = (cartline.ChargeLines || []).filter(
          (chargeLine) => chargeLine.IsShipping
        );
        const chargeLines = cartline.ChargeLines || 0;
        if (chargeLines[0]) {
          isShippingCalculated = true;
          freightFee =
            freightFee +
            shippingChargeLines.reduce((chargeTotal, chargeLine) => {
              return (
                chargeTotal + (chargeLine.NetAmountWithAllInclusiveTax || 0)
              );
            }, 0);
        }
      });
  }
  return (
    <p className={`msc-order-summary__line-shipping`}>
      <span className="msc-order-summary__label">
        {props.resources.shippingLabel}
      </span>
      {isShippingCalculated && freightFee ? (
        <PriceComponent
          data={{
            price: { CustomerContextualPrice: freightFee },
          }}
          freePriceText={undefined}
          context={props.context}
          id={props.id}
          typeName={props.typeName}
          className={"msc-order-summary__value"}
        />
      ) : (
        <span className="msc-order-summary__value">
          {props.resources.toBeCalculatedText}
        </span>
      )}
    </p>
  );
}

const OrderSummaryComponent: React.FC<ICheckoutViewProps> = (
  props: ICheckoutViewProps
) => {
  const { heading, lines } = props.orderSummary!;
  const { request } = props.context;
  const { channelDeliveryOptionConfig } = props?.data;
  const cartLines =
    props.data.checkout.result?.checkoutCart.cart.CartLines || [];
  const pickupDeliveryModeCode =
    request && request.channel && request.channel.PickupDeliveryModeCode;
  const emailDeliveryModeCode =
    request && request.channel && request.channel.EmailDeliveryModeCode;
  const multiplePickupStoreSwitchName =
    "Dynamics.AX.Application.RetailMultiplePickupDeliveryModeFeature";
  const retailMultiplePickUpOptionEnabled = props.data.featureState?.result?.find(
    (item) => item.Name === multiplePickupStoreSwitchName
  )?.IsEnabled;
  const deliveryModes = cartLines.map(
    (cartLine: CartLine) => cartLine.DeliveryMode
  );
  const canShip = deliveryModes.some(
    (deliveryMode: string | undefined) =>
      !(
        deliveryMode !== "" &&
        (deliveryMode ===
          getDeliveryMode(
            deliveryMode,
            retailMultiplePickUpOptionEnabled,
            channelDeliveryOptionConfig.result,
            pickupDeliveryModeCode
          ) ||
          deliveryMode === emailDeliveryModeCode)
      )
  );
  return (
    <div className="msc-order-summary-wrapper">
      {heading}
      <div className="msc-order-summary__items">
        {lines && (
          <>
            {_renderSubTotal(props)}
            {canShip ? _renderShipping(props) : lines.shipping}
            {lines.otherCharge}
            {lines.tax}
            {lines.totalDiscounts}
            {lines.loyalty}
            {lines.giftCard}
            {lines.orderTotal}
          </>
        )}
      </div>
    </div>
  );
};

const PaymentSummaryComponent: React.FC<IInvoicePaymentSummary> = ({
  heading,
  lines,
}) => (
  <div className="msc-invoice-summary-wrapper">
    {heading}
    <div className="msc-invoice-summary__items">
      {lines && (
        <>
          {lines.invoices}
          {lines.giftCard}
          {lines.loyalty}
          {lines.orderTotal}
        </>
      )}
    </div>
  </div>
);

const CheckoutView: React.FC<ICheckoutViewProps> = (props) => {
  const {
    canShow,
    checkoutProps,
    headerProps,
    hasSalesOrder,
    hasInvoiceLine,
    bodyProps,
    mainProps,
    mainControlProps,
    sideProps,
    sideControlFirstProps,
    sideControlSecondProps,
    termsAndConditionsProps,
    orderConfirmation,
    loading,
    alert,
    title,
    guidedForm,
    orderSummary,
    invoicePaymentSummary,
    lineItems,
    placeOrderButton,
    termsAndConditions,
    keepShoppingButton,
  } = props;

  return (
    <Module {...checkoutProps}>
      {!hasSalesOrder && <Node {...headerProps}>{title}</Node>}
      {!hasSalesOrder && (
        <Node {...bodyProps}>
          {loading}
          {alert}
          {canShow && (
            <>
              <Node {...mainProps}>
                {guidedForm}
                <Node {...termsAndConditionsProps}>{termsAndConditions}</Node>
                <Node {...mainControlProps}>
                  {placeOrderButton}
                  {keepShoppingButton}
                </Node>
              </Node>
              <Node {...sideProps}>
                {!hasInvoiceLine
                  ? orderSummary && <OrderSummaryComponent {...props} />
                  : invoicePaymentSummary && (
                      <PaymentSummaryComponent {...invoicePaymentSummary} />
                    )}
                <Node {...sideControlFirstProps}>
                  <Node {...termsAndConditionsProps}>{termsAndConditions}</Node>
                  {placeOrderButton}
                  {keepShoppingButton}
                </Node>
                {lineItems && <LineItemsComponent {...lineItems} />}
                <Node {...sideControlSecondProps}>
                  <Node {...termsAndConditionsProps}>{termsAndConditions}</Node>
                  {placeOrderButton}
                  {keepShoppingButton}
                </Node>
              </Node>
            </>
          )}
        </Node>
      )}
      {hasSalesOrder && orderConfirmation}
    </Module>
  );
};

export default CheckoutView;
