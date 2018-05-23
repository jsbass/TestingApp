import { IDataProtector } from "../../dataprotection/abstractions/IDataProtector";
import { CookieOptions, Response } from "express";
import { Identity } from "./Identity";

export class AuthHelper<T extends object> {
    private readonly dataProtector: IDataProtector;
    private readonly cookieName: string;
    private readonly cookieOptions: CookieOptions;
    private readonly authenticateUserFunc: (cookieObject: T) => boolean;
    constructor(dataProtector: IDataProtector, cookieName: string, cookieOptions: CookieOptions, authenticate: (cookieObject: T) => boolean) {
        this.dataProtector = dataProtector;
        this.cookieName = cookieName;
        this.cookieOptions = cookieOptions;
        this.authenticateUserFunc = authenticate;
    }

    public updateAuthCookie(response: Response, user: T) {
        if(user == null) {
            response.clearCookie(this.cookieName);
            return;
        }
        let encrypted = this.dataProtector.protect(Buffer.from(JSON.stringify(user)));
        response.cookie(this.cookieName, encrypted.toString('base64'), this.cookieOptions);
    }

    public getMiddlware() {
        return (req, res, next) => {
            console.log('looking to deserialize auth from cookie...')
            let map: Map<string, string> = new Map<string, string>();
            (req.get("cookie") || "").split('; ').forEach((c) => {
                let split = c.split('=');
                map.set(split[0], split[1]);
            });
            let cookies: Map<string, string> = map;
            let identity = new Identity<T>(this.authenticateUserFunc);
            let cookie = decodeURIComponent(cookies.get(this.cookieName));
            if(cookie != null) {
                let unencrypted = (this.dataProtector.unprotect(Buffer.from(cookie, 'base64')) || Buffer.alloc(1)).toString();
                var user: T;
                try {
                    user = JSON.parse(unencrypted) as T;
                }
                catch {
                    user = null;
                }
                if(user != null) {
                    identity.user = user;
                }
            } else {
                identity.user = null;
            }

            req.identity = identity;
            this.updateAuthCookie(res, req.identity.user);
            next();
        };
    }
}