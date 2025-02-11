<script>
    import { Table, TableBody, TableBodyCell, TableBodyRow, TableHead, TableHeadCell } from 'flowbite-svelte';

    let { data } = $props();
</script>

<Table>
    <TableHead>
        <TableHeadCell>Product name</TableHeadCell>
        <TableHeadCell>Color</TableHeadCell>
        <TableHeadCell>Category</TableHeadCell>
        <TableHeadCell>Price</TableHeadCell>
    </TableHead>
    <TableBody tableBodyClass="divide-y">
        {#each data.members as member}
            <TableBodyRow>
                <TableBodyCell>Apple MacBook Pro 17"</TableBodyCell>
                <TableBodyCell>Sliver</TableBodyCell>
                <TableBodyCell>Laptop</TableBodyCell>
                <TableBodyCell>$2999</TableBodyCell>
            </TableBodyRow>
        {/each}

    </TableBody>
</Table>