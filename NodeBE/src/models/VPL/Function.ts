export class Function {
    name: string;
    id: string;
    inputs: Port[];
    outputs: Port[];
    code: string;
}

export class Port {
    name: string;
    type: string;
}