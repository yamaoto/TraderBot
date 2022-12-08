// @generated by protobuf-ts 2.8.2
// @generated from protobuf file "mail_box.proto" (package "mail_box", syntax proto3)
// tslint:disable
import type { RpcTransport } from "@protobuf-ts/runtime-rpc";
import type { ServiceInfo } from "@protobuf-ts/runtime-rpc";
import { MailBoxGrpc } from "./mail_box";
import type { DeleteMailBoxResponse } from "./mail_box";
import type { DeleteMailBoxRequest } from "./mail_box";
import type { UpsertMailBoxResponse } from "./mail_box";
import type { MailBoxSettings } from "./mail_box";
import { stackIntercept } from "@protobuf-ts/runtime-rpc";
import type { GetMailBoxesResponse } from "./mail_box";
import type { Empty } from "./google/protobuf/empty";
import type { UnaryCall } from "@protobuf-ts/runtime-rpc";
import type { RpcOptions } from "@protobuf-ts/runtime-rpc";
/**
 * @generated from protobuf service mail_box.MailBoxGrpc
 */
export interface IMailBoxGrpcClient {
    /**
     * @generated from protobuf rpc: GetMailBoxes(google.protobuf.Empty) returns (mail_box.GetMailBoxesResponse);
     */
    getMailBoxes(input: Empty, options?: RpcOptions): UnaryCall<Empty, GetMailBoxesResponse>;
    /**
     * @generated from protobuf rpc: UpsertMailBox(mail_box.MailBoxSettings) returns (mail_box.UpsertMailBoxResponse);
     */
    upsertMailBox(input: MailBoxSettings, options?: RpcOptions): UnaryCall<MailBoxSettings, UpsertMailBoxResponse>;
    /**
     * @generated from protobuf rpc: DeleteMailBox(mail_box.DeleteMailBoxRequest) returns (mail_box.DeleteMailBoxResponse);
     */
    deleteMailBox(input: DeleteMailBoxRequest, options?: RpcOptions): UnaryCall<DeleteMailBoxRequest, DeleteMailBoxResponse>;
}
/**
 * @generated from protobuf service mail_box.MailBoxGrpc
 */
export class MailBoxGrpcClient implements IMailBoxGrpcClient, ServiceInfo {
    typeName = MailBoxGrpc.typeName;
    methods = MailBoxGrpc.methods;
    options = MailBoxGrpc.options;
    constructor(private readonly _transport: RpcTransport) {
    }
    /**
     * @generated from protobuf rpc: GetMailBoxes(google.protobuf.Empty) returns (mail_box.GetMailBoxesResponse);
     */
    getMailBoxes(input: Empty, options?: RpcOptions): UnaryCall<Empty, GetMailBoxesResponse> {
        const method = this.methods[0], opt = this._transport.mergeOptions(options);
        return stackIntercept<Empty, GetMailBoxesResponse>("unary", this._transport, method, opt, input);
    }
    /**
     * @generated from protobuf rpc: UpsertMailBox(mail_box.MailBoxSettings) returns (mail_box.UpsertMailBoxResponse);
     */
    upsertMailBox(input: MailBoxSettings, options?: RpcOptions): UnaryCall<MailBoxSettings, UpsertMailBoxResponse> {
        const method = this.methods[1], opt = this._transport.mergeOptions(options);
        return stackIntercept<MailBoxSettings, UpsertMailBoxResponse>("unary", this._transport, method, opt, input);
    }
    /**
     * @generated from protobuf rpc: DeleteMailBox(mail_box.DeleteMailBoxRequest) returns (mail_box.DeleteMailBoxResponse);
     */
    deleteMailBox(input: DeleteMailBoxRequest, options?: RpcOptions): UnaryCall<DeleteMailBoxRequest, DeleteMailBoxResponse> {
        const method = this.methods[2], opt = this._transport.mergeOptions(options);
        return stackIntercept<DeleteMailBoxRequest, DeleteMailBoxResponse>("unary", this._transport, method, opt, input);
    }
}