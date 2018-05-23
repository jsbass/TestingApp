import { IDataProtector } from "../../dataprotection/abstractions/IDataProtector";
import { Request, Response, NextFunction, Handler, CookieOptions } from 'express';
import { Identity } from "./Identity";

export function authMiddleware<T extends object>(dataProtector: IDataProtector, cookieName: string, loginUrl: string, cookieOptions: CookieOptions, authenticate: (cookieObject: T) => boolean): Handler {
    return (req: any, res, next: NextFunction): any => {
        console.log('looking to deserialize auth from cookie...')
        let map: Map<string, string> = new Map<string, string>();
        (req.get("cookie") || "").split('; ').forEach((c) => {
            let split = c.split('=');
            map.set(split[0], split[1]);
        });
        let cookies: Map<string, string> = map;
        let identity = new Identity<T>(authenticate);
        let cookie = cookies.get(cookieName);
        if(cookie != null) {
            let unencrypted = dataProtector.unprotect(Buffer.from(cookie, 'base64')).toString();
            var user: T = JSON.parse(unencrypted) as T;
            if(user != null) {
                req.identity.user = user;
            }
        } else {
            identity.user = null;
        }
        req.identity = identity;
        let encrypted = dataProtector.protect(Buffer.from(JSON.stringify(req.identity.user)));
        res.cookie(cookieName, encrypted, cookieOptions);
        next();
    };
}