import { Element } from "./Element";

export class Group extends Element {
    type = "Group";
    name: string;
    children: Element[];
}