<script lang="ts">
	import '../app.css';
    import NavBar from '$lib/components/nav-bar.svelte';
    import {title} from "$lib/config";

	let { children } = $props();
</script>

<svelte:head>
    <title>{title}</title>
</svelte:head>

<div class="flex flex-col justify-between h-screen">
    <NavBar />
    <main class="grow">
        <div class="container mx-auto">
            {@render children()}
        </div>

    </main>

</div>

