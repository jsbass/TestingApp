import { Element } from "./Element";

export class Node extends Element {
    type = "Node";
    functionType: FunctionType;
    function: Function | string;
    inputs: Map<string, Connection>;
}

export enum FunctionType {
    Inline = "Inline",
    Stored = "Stored"
}

export class Connection {
    portName: string;
    nodeId: string;
}