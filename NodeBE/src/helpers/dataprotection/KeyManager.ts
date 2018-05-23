import { IKeyManager } from "./abstractions/IKeyManager";
import { IKey } from "./abstractions/IKey";
import { IKeyStore } from "./abstractions/IKeyStore";
export class KeyManager implements IKeyManager {
    private _keyStore: IKeyStore;
    private _keys

    private readonly KEYTIMEOUT = 1000*60*60*24*3;
    constructor(keyStore: IKeyStore) {
        this._keyStore = keyStore;
    }

    get(): IKey {
        let activeKeys = this._keyStore.getAll().filter((k, i, arr) => k.isRevoked);
        if(activeKeys.length == 0) {
            //no active keys exist
            //create a new one
            return this._keyStore.create(this.KEYTIMEOUT);
        }
        //key that expires the latest
        return activeKeys.sort((a, b) => (a.expires.getDate() - b.expires.getDate()))[0];
    }

    getById(id: number): IKey {
        let key = this._keyStore.get(id);
        // don't return keys that have been expired for more than the timeout
        if(key != null && key.isRevoked) {
            if((key.expires.getDate() - Date.now()) > this.KEYTIMEOUT) {
                return null;
            }
        }

        return key;
    }

    
}