<script>
    import { onMount } from "svelte";
    import { MailBoxGrpcClient } from "../admin-client/mail_box.client";
    import { GrpcWebFetchTransport } from "@protobuf-ts/grpcweb-transport";
    import MailBox from "./MailBox.svelte";

    const transport = new GrpcWebFetchTransport({
        baseUrl: "http://localhost:5258",
    });
    const client = new MailBoxGrpcClient(transport);

    let mailBoxes = [];
    let lastUpdatedAt = "...loading";
    let health = true;
    let newMailBox = {
        name: "",
        username: "",
        password: "",
        binanceApiKey: "",
        binanceApiSecret: "",
        allowedCopyFrom: [],
    };

    const load = async () => {
        let getMailBoxesResponse;
        try {
            getMailBoxesResponse = await client.getMailBoxes(null, null);
        } catch (err) {
            health = false;
            return;
        }
        mailBoxes = getMailBoxesResponse.response.items;
        lastUpdatedAt = new Date().toLocaleString();
        health = true;
    };

    onMount(async () => await load());

    setInterval(() => load().then(), 5000);

    const onSave = async (eventData) => {
        const data = eventData.detail;
        let upsertMailBoxResponse;
        try {
            upsertMailBoxResponse = await client.upsertMailBox(
                {
                    name: data.name,
                    username: data.username,
                    password: data.password,
                    binanceApiKey: data.binanceApiKey,
                    binanceApiSecret: data.binanceApiSecret,
                    allowedCopyFrom: data.allowedCopyFrom,
                },
                null
            );
            if (!upsertMailBoxResponse.response.result) {
                console.log("ErrorCode: ", upsertMailBoxResponse.response.errorCode);
                console.log("Error: ", upsertMailBoxResponse.response.errorMessage);
                alert("ErrorCode: " + upsertMailBoxResponse.response.errorCode);
                return;
            }
            await load();
        } catch (err) {
            health = false;
            alert("Error. See console for details");
            console.log("MailBox save error", err);
        }
    };

    const onDelte = async (eventData) => {
        const data = eventData.detail;
        let deleteMailBoxResponse;
        try {
            deleteMailBoxResponse = await client.deleteMailBox(
                {
                    name: data.name,
                },
                null
            );
            if (!deleteMailBoxResponse.response.result) {
                console.log("ErrorCode: ", deleteMailBoxResponse.response.errorCode);
                console.log("Error: ", deleteMailBoxResponse.response.errorMessage);
                alert("ErrorCode: " + deleteMailBoxResponse.response.errorCode);
                return;
            }
            await load();
        } catch (err) {
            health = false;
            console.log("MailBox delete error", err);
        }
    };
</script>

<h1 class:error-text-red={!health}>Mail Boxes</h1>

{#if health}
    <div class="update-indicator">
        Actual at <a title="Reload" href={null} on:click={load}>{lastUpdatedAt}</a>
    </div>
{:else}
    <div class="error">Communication error</div>
    <a href={null} on:click={load}>Try again</a>
{/if}

<div class="mail-boxes">
    <div class="new">
        <MailBox mailBox={newMailBox} title="Create new" edit={true} on:save={onSave} />
    </div>
    {#each mailBoxes as mailBox}
        <MailBox {mailBox} title="" on:save={onSave} on:delete={onDelte} />
    {/each}
</div>

<style>
    .mail-boxes {
        display: flex;
        flex-direction: column;
        flex-basis: 250px;
    }
</style>
