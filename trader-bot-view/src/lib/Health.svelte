<script>
    import { onMount } from "svelte";
    import axios from "axios";

    let lastUpdatedAt = "...loading";
    let health = true;

    const setStatusText = (health) => {
        switch (health) {
            case null:
                return "👀 No information";
            case "Healthy":
                return "✅ Processing";
            default:
                return "💔 Unhealthy";
        }
    };

    let adminHealth = setStatusText();
    let emailListenerHealth = setStatusText();
    let binanceConnectHealth = setStatusText();
    let orderControllerHealth = setStatusText();

    const load = () => {
        lastUpdatedAt = new Date().toLocaleString();
        axios
            .get("http://localhost:5258/health")
            .then((response) => (adminHealth = setStatusText(response.data)));
        axios
            .get("http://localhost:5226/health")
            .then((response) => (emailListenerHealth = setStatusText(response.data)));
        axios
            .get("http://localhost:5167/health")
            .then((response) => (binanceConnectHealth = setStatusText(response.data)));
        axios
            .get("http://localhost:5111/health")
            .then((response) => (orderControllerHealth = setStatusText(response.data)));
    };

    onMount(() => load());

    setInterval(() => load(), 5000);
</script>

<div class="root">
    <h1 class:error-text-red={!health}>Health</h1>

    {#if health}
        <div class="update-indicator">
            Actual at <a title="Reload" href={null} on:click={load}>{lastUpdatedAt}</a>
        </div>
    {:else}
        <div class="error">Communication error</div>
        <a href={null} on:click={load}>Try again</a>
    {/if}

    <div class="health">
        <p><span class="service">EmailListener</span>: {emailListenerHealth}</p>
        <p><span class="service">Admin</span>: {adminHealth}</p>
        <p><span class="service">BinanceConnect</span>: {binanceConnectHealth}</p>
        <p><span class="service">OrderController</span>: {orderControllerHealth}</p>
    </div>
</div>

<style>
    .root {
        text-align: left;
    }
    .health {
        display: flex;
        flex-direction: column;
    }
    .service {
        width: 150px;
        display: inline-block;
    }
</style>
