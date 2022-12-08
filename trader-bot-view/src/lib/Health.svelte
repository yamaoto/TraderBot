<script>
    import { onMount } from "svelte";

    let lastUpdatedAt = "...loading";
    let health = true;

    const load = async () => {};

    onMount(async () => await load());

    setInterval(() => load().then(), 5000);
</script>

<div class="root">
    <h1 class:error-text-red={!health}>Health</h1>

    {#if health}
        <div class="update-indicator">
            Actual at <a title="Reload" href="javascript:void(0)" on:click={load}>{lastUpdatedAt}</a>
        </div>
    {:else}
        <div class="error">Communication error</div>
        <a href="javascript:void(0)" on:click={load}>Try again</a>
    {/if}

    <div class="health">
        <p><span class="service">EmailListener</span>: 👀 No information</p>
        <p><span class="service">Admin</span>: ✅ Processing</p>
        <p><span class="service">BinanceConnect</span>: 💔 Unhealthy</p>
        <p><span class="service">OrderController</span>: 👀 No information</p>
        <p><span class="service">RavenDb</span>: 👀 No information</p>
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
