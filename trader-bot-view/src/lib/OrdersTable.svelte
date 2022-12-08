<script>
    import { onMount } from "svelte";
    import { AdminGrpcClient } from "../admin-client/admin.client";
    import { GrpcWebFetchTransport } from "@protobuf-ts/grpcweb-transport";

    const transport = new GrpcWebFetchTransport({
        baseUrl: "http://localhost:5258",
    });
    const client = new AdminGrpcClient(transport);

    let orders = [];
    let lastUpdatedAt = "...loading";
    let health = true;

    const load = async () => {
        let getOrdersResponse;
        try {
            getOrdersResponse = await client.getOrders(null, null);
        } catch (err) {
            health = false;
            return;
        }
        orders = getOrdersResponse.response.orders.map((order) => ({
            ...order,
            createdAtDate: new Date(order.createdAt).toLocaleString(),
        }));
        lastUpdatedAt = new Date().toLocaleString();
        health = true;
    };

    onMount(async () => await load());

    setInterval(() => load().then(), 5000);
</script>

<h1 class:error-text-red={!health}>Orders</h1>

{#if health}
    <div class="update-indicator">
        Actual at <a title="Reload" href={null} on:click={load}>{lastUpdatedAt}</a>
    </div>
{:else}
    <div class="error">Communication error</div>
    <a href={null} on:click={load}>Try again</a>
{/if}
<div class="orders">
    {#each orders as order}
        <div class="active-element">
            <p>Copy order from {order.from} @ {order.createdAtDate}</p>
            <p>{order.orderSide} {order.tradingSymbol}</p>
            <p>Price: {order.price}</p>
            <p>Quantity: {order.quantity}</p>
            <p>Status: {order.status}</p>
        </div>
    {/each}
</div>

<style>
    .orders {
        display: flex;
        flex-direction: column;
        flex-basis: 250px;
    }
</style>
