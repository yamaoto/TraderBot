<script>
    import { onMount, afterUpdate } from "svelte";
    import { createEventDispatcher } from "svelte";

    export let mailBox;
    export let edit = false;
    export let title;

    let mailBoxEdit = {
        name: "",
        username: "",
        password: "",
        binanceApiKey: "",
        binanceApiSecret: "",
        allowedCopyFrom: [],
    };

    onMount(() => {
        mailBoxEdit = {
            ...mailBox,
        };
    });

    const dispatch = createEventDispatcher();

    const onSave = (e) => {
        edit = false;
        dispatch("save", {
            ...mailBoxEdit,
        });
        mailBoxEdit = {
            ...mailBox,
        };
        e.preventDefault();
    };
    const onCancel = ()=>{
        mailBoxEdit = {
            ...mailBox,
        };
        edit = false;
    }

    const onDelete = ()=>{
        dispatch("delete", {
            ...mailBoxEdit,
        });
    }
    const turnEdit = () => {
        mailBoxEdit = {
            ...mailBox,
        };
        edit = true;
    };
</script>

{#if edit}
    <div class="edit active-element">
        {title}
        <form on:submit={onSave}>
            <div class="form-field">
                <label for="name">Name: </label>
                <input name="name" type="text" bind:value={mailBoxEdit.name} />
            </div>
            <div class="form-field">
                <label for="username">Username: </label>
                <input name="username" type="text" bind:value={mailBoxEdit.username} />
            </div>
            <div class="form-field">
                <label for="password">Password: </label>
                <input name="password" type="password" bind:value={mailBoxEdit.password} />
            </div>
            <div class="form-field">
                <label for="binanceApiKey">Binance API Key: </label>
                <input name="binanceApiKey" type="password" bind:value={mailBoxEdit.binanceApiKey} />
            </div>
            <div class="form-field">
                <label for="binanceApiSecret">Binance API Secret: </label>
                <input name="binanceApiSecret" type="password" bind:value={mailBoxEdit.binanceApiSecret} />
            </div>
            <div class="actions">
                <button on:click={onSave}>Save</button>
                <button on:click={onCancel}>Cancel</button>
                <button on:click={onDelete}>Delete</button>
            </div>
        </form>
    </div>
{:else}
    <div class="view">
        MailBox: {mailBox.name} ({mailBox.username}) <a href={null} on:click={turnEdit}>edit</a>
    </div>
{/if}

<style>
    .form-field {
        text-align: left;
    }
    .form-field input {
        display: block;
        width: 100%;
    }
    .actions {
        display: flex;
        flex-direction: row;
    }
    .actions button {
        margin-top: 10px;
        flex: auto;
    }
</style>