import { IDataProtector, IDataProtectorContructor } from "./abstractions/IDataProtector";
import { DataProtector } from "./DataProtector";
import { KeyManager } from "./KeyManager";
import { IKeyStore } from "./abstractions/IKeyStore";

export class ProtectionProvider {
    private protectors: { [purpose: string]: IDataProtector } = {};
    private _keyStore: IKeyStore;
    constructor(keystore: IKeyStore) {
        this._keyStore = keystore;
    }

    getProtector(purpose: string): IDataProtector {
        if(this.protectors[purpose] == null) {
            this.protectors[purpose] = new DataProtector(purpose, this._keyStore);
        }

        return this.protectors[purpose];
    }
}