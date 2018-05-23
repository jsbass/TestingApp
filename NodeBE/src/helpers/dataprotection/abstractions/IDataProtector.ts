import { IKeyManagerConstructor } from "./IKeyManager";

export interface IDataProtector {
    protect(unencrypted: Buffer): Buffer;

    unprotect(encrypted: Buffer): Buffer;
}

export interface IDataProtectorContructor {
    new (purpose: string, ctor: IKeyManagerConstructor): IDataProtectorContructor;
}