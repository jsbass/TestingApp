import { IKeyStore } from "./abstractions/IKeyStore";
import { IKey, IKeyConstructor } from "./abstractions/IKey";
import { Key } from "./Key";
import * as crypto from 'crypto';

export class MemoryKeyStore implements IKeyStore {
    private _keys: Map<number, IKey> = new Map<number, IKey>();
    
    create(duration: number): IKey {
        let counter = 0;
        let key: Key;
        do {
            key = Key.generateNewKey();
            if(counter++ > 100) {
                throw "took too many tries to generate an unused key id"
            }
        } while(this._keys[key.id] != null)
        key.expires = new Date(Date.now() + duration);
        this._keys.set(key.id, key);
        return key;
    }
    revoke(id: number, reason: string) {
        let key = this._keys.get(id);
        if(key == null) {
            return;
        }
        key.expires = new Date();
    }

    getAll(): IKey[] {
        return Array.from(this._keys.values());
    }

    revokeAll(reason: string) {
        this._keys.forEach((key, id, map) => {
            this.revoke(key.id, reason);
        });
    }
    get(id: number): IKey {
        return this._keys.get(id);
    }
}