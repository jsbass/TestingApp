import { IKey, IKeyConstructor } from './IKey';
export interface IKeyStore {
    create(duration: number): IKey;
    revoke(id: number, reason: string);
    getAll(): IKey[];
    revokeAll(reason: string);
    get(id: number): IKey;
}

export interface IKeyStoreConstructor {
    new (ctor: IKeyConstructor): IKey;
}