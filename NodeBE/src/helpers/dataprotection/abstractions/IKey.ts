export interface IKey {
    id: number;
    value: string;
    created: Date;
    expires: Date;
    isRevoked: boolean;
}

export interface IKeyConstructor {
    new (): IKey;
}