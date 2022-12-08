<script>
    import Health from "./lib/Health.svelte";
    import MailBoxes from "./lib/MailBoxes.svelte";
    import OrdersTable from "./lib/OrdersTable.svelte";
    import Tips from "./lib/Tips.svelte";
</script>

<main>
    <div class="main-content flex-container">
        <div>
            <Tips />
        </div>
        <div>
            <MailBoxes />
        </div>
        <div>
            <OrdersTable />
        </div>
    </div>
    <footer>
        <hr />

        <Health />
    </footer>
</main>

<style>
    .flex-container {
        display: flex;
        flex-direction: row;
    }

    .flex-container > div {
        width: 300px;
        margin: 0 50px;
    }

    .main-content {
        min-height: 100vh;
    }
</style>
