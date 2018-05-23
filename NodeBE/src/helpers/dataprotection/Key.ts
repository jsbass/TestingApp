import { IKey } from "./abstractions/IKey";
import * as crypto from 'crypto';

export class Key implements IKey {
    id: number;
    value: string;
    created: Date;
    expires: Date;
    
    public static readonly KEY_ID_BYTES_SIZE = 2;
    
    get isRevoked(): boolean {
        return this.expires.getDate() > Date.now();
    }

    public static generateNewKey(): Key {
        let key = new Key();
        key.id = crypto.randomBytes(this.KEY_ID_BYTES_SIZE).readUInt16BE(0);
        key.value = crypto.randomBytes(32)
            .toString('base64');
        return key;
    }
}