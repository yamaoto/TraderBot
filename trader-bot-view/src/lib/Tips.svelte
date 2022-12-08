<script>
    import SvelteMarkdown from "svelte-markdown";
    const source = `
## TraderBot

Trader Bot purpose is to copy future orders from following traders from TraderWagon without TraderWagon commission.

Process:

1. Follow trader on TraderWagon with minimal commission rate
2. Get E-Mail notification from TraderWagon and TraderBot will parse and create new order with custom quantity in
   Binance`;
</script>

<h1>Trader copy bot</h1>

<div class="tips">
    <SvelteMarkdown {source} />

    <h2>Tutorial</h2>
    <p>1. Setup Mail box with Gmail account</p>
    <p>2. Examine orders</p>
</div>

<style>
    .tips {
        text-align: left;
    }
</style>
