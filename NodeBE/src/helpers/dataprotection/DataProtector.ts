import { IDataProtector } from "./abstractions/IDataProtector";
import { IKeyManager, IKeyManagerConstructor } from "./abstractions/IKeyManager";
import { KeyManager } from "./KeyManager";
import * as crypto from 'crypto';
import { IKeyStore } from "./abstractions/IKeyStore";
import { Key } from "./Key";

export class DataProtector implements IDataProtector {  
    private _keyManager: IKeyManager;
    private _purpose: string;
    private ks: IKeyStore;
    private readonly MAGIC_BYTES = 0xFFFF;
    private readonly MAGIC_BYTES_SIZE = 4;
    private readonly IV_SIZE = 32;
    private get MAGIC_BUFFER(): Buffer {
        let buffer = new Buffer(this.MAGIC_BYTES_SIZE);
        buffer.writeIntBE(this.MAGIC_BYTES, 0, this.MAGIC_BYTES_SIZE);
        return buffer;
    }

    constructor(purpose: string, keyStore: IKeyStore) {
        this.ks = keyStore;
        this._purpose = purpose;
        this._keyManager = new KeyManager(keyStore);
    }
    
    protect(unencrypted: Buffer): Buffer {
        let key = this._keyManager.get();
        let keyIdBytes = new Buffer(Key.KEY_ID_BYTES_SIZE);
        keyIdBytes.writeUIntBE(key.id, 0, Key.KEY_ID_BYTES_SIZE);
        let aad = Buffer.concat([this.MAGIC_BUFFER, keyIdBytes, Buffer.from(this._purpose)])
        let iv = crypto.randomBytes(this.IV_SIZE);
        
        let cipher = crypto.createCipheriv('aes-256-gcm', Buffer.from(key.value, 'base64'), iv);
        cipher.setAAD(aad);
        let encrypted = Buffer.concat([cipher.update(unencrypted),cipher.final()]);
        let authTag = cipher.getAuthTag();
        console.debug('encrypting');
        console.debug(`keyid: ${key.id}:${keyIdBytes.toString('base64')}`);
        console.debug(`magic: ${this.MAGIC_BYTES}`);
        console.debug(`purpose: ${this._purpose}`);
        console.debug(`iv: ${iv.toString('base64')}`);
        console.debug(`aad: ${aad.toString('base64')}`);
        console.debug(`authTag: ${authTag.toString('base64')}`);
        console.debug(`encryptedData: ${encrypted.toString('base64')}`)
        console.debug('encrypted:');
        console.debug(Buffer.concat([this.MAGIC_BUFFER, keyIdBytes, iv, encrypted, authTag]).toString('base64'));
        return Buffer.concat([this.MAGIC_BUFFER, keyIdBytes, iv, encrypted, authTag]);
    }

    unprotect(encrypted: Buffer): Buffer {
        try{
            let magic_bytes = encrypted.readIntBE(0, this.MAGIC_BYTES_SIZE);
            let keyIdBytes = encrypted.slice(this.MAGIC_BYTES_SIZE, this.MAGIC_BYTES_SIZE + Key.KEY_ID_BYTES_SIZE)
            let keyId = keyIdBytes.readUIntBE(0, Key.KEY_ID_BYTES_SIZE);
            let key = this._keyManager.getById(keyId);
            let iv = encrypted.slice(this.MAGIC_BYTES_SIZE+Key.KEY_ID_BYTES_SIZE, this.MAGIC_BYTES_SIZE+Key.KEY_ID_BYTES_SIZE+this.IV_SIZE);
            let aad = Buffer.concat([this.MAGIC_BUFFER, keyIdBytes, Buffer.from(this._purpose)]);
            let authTag = encrypted.slice(encrypted.length-16, encrypted.length);
            let encryptedData = encrypted.slice(this.MAGIC_BYTES_SIZE+Key.KEY_ID_BYTES_SIZE+this.IV_SIZE, encrypted.length-16);
            console.debug(this.ks.getAll());
            console.debug('decrypting:');
            console.debug(encrypted.toString('base64'));
            console.debug(`keyid: ${keyId}:${keyIdBytes.toString('base64')}`);
            console.debug(`magic: ${magic_bytes}`);
            console.debug(`purpose: ${this._purpose}`);
            console.debug(`iv: ${iv.toString('base64')}`);
            console.debug(`aad: ${aad.toString('base64')}`);
            console.debug(`authTag: ${authTag.toString('base64')}`);
            console.debug(`encryptedData: ${encryptedData.toString('base64')}`);
            let decipher = crypto.createDecipheriv('aes-256-gcm', Buffer.from(key.value, 'base64'), iv);
            decipher.setAAD(aad);
            decipher.setAuthTag(authTag);
            let decrypted = decipher.update(encryptedData);
            return Buffer.concat([decrypted, decipher.final()]);
        }
        catch(e) {
            console.error(e.toString());
        }

        return null;
    }
}