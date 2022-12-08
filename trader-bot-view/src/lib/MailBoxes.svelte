<script>
    import { onMount } from "svelte";

    let lastUpdatedAt = "...loading";
    let health = true;

    const load = async () => {};

    onMount(async () => await load());

    setInterval(() => load().then(), 5000);
</script>

<h1 class:error-text-red={!health}>Mail Boxes</h1>

{#if health}
    <div class="update-indicator">
        Actual at <a title="Reload" href="javascript:void(0)" on:click={load}>{lastUpdatedAt}</a>
    </div>
{:else}
    <div class="error">Communication error</div>
    <a href="javascript:void(0)" on:click={load}>Try again</a>
{/if}

<div class="mail-boxes" />

<style>
    .mail-boxes {
        display: flex;
        flex-direction: column;
        flex-basis: 250px;
    }
</style>
