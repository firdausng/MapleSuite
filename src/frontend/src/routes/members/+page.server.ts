import type {ServerLoadEvent} from "@sveltejs/kit";
import {apis} from "$lib/config";

export async function load({ fetch }: ServerLoadEvent) {
    const response = await fetch(`${apis.member}/members`);
    const members = await response.json();
    return { members }
}