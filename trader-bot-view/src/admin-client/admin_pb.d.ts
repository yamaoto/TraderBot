// package: admin
// file: admin.proto

import * as jspb from "google-protobuf";
import * as google_protobuf_empty_pb from "google-protobuf/google/protobuf/empty_pb";

export class Order extends jspb.Message {
  getId(): string;
  setId(value: string): void;

  getTradingSymbol(): string;
  setTradingSymbol(value: string): void;

  getOrderSide(): string;
  setOrderSide(value: string): void;

  getPrice(): string;
  setPrice(value: string): void;

  getQuantity(): string;
  setQuantity(value: string): void;

  getFrom(): string;
  setFrom(value: string): void;

  getCreatedat(): string;
  setCreatedat(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Order.AsObject;
  static toObject(includeInstance: boolean, msg: Order): Order.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: Order, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Order;
  static deserializeBinaryFromReader(message: Order, reader: jspb.BinaryReader): Order;
}

export namespace Order {
  export type AsObject = {
    id: string,
    tradingSymbol: string,
    orderSide: string,
    price: string,
    quantity: string,
    from: string,
    createdat: string,
  }
}

export class GetOrdersResponse extends jspb.Message {
  clearOrdersList(): void;
  getOrdersList(): Array<Order>;
  setOrdersList(value: Array<Order>): void;
  addOrders(value?: Order, index?: number): Order;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetOrdersResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetOrdersResponse): GetOrdersResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetOrdersResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetOrdersResponse;
  static deserializeBinaryFromReader(message: GetOrdersResponse, reader: jspb.BinaryReader): GetOrdersResponse;
}

export namespace GetOrdersResponse {
  export type AsObject = {
    ordersList: Array<Order.AsObject>,
  }
}

