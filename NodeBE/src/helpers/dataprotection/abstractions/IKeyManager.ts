import { IKey } from "./IKey";
import { IKeyStoreConstructor } from "./IKeyStore";

export interface IKeyManager {
    get(): IKey
    getById(id: number): IKey;
}

export interface IKeyManagerConstructor {
    new (ctor: IKeyStoreConstructor): IKeyManager;
}