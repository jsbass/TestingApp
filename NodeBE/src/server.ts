import * as express from 'express';
import * as bodyParser from 'body-parser';
import { CookieOptions } from 'express';
import { ProtectionProvider } from './helpers/dataprotection/ProtectionProvider';
import { MemoryKeyStore } from './helpers/dataprotection/MemoryKeyStore';
import { DataProtector } from './helpers/dataprotection/DataProtector';
import { authMiddleware } from './helpers/middleware/auth/auth-middleware';
import { User } from './models/User';
import * as fs from 'fs';
import {} from 'os'
import { AuthHelper } from './helpers/middleware/auth/auth-helper';

let keyStore = new MemoryKeyStore();
let provider = new ProtectionProvider(keyStore);
let protector = provider.getProtector("testing");
let testString = "asdfqwer";
console.log(`testString: ${testString}`);
let encrypted = protector.protect(Buffer.from(testString));
console.log(`encrypted: ${encrypted.toString('base64')}`);
let decrypted = protector.unprotect(encrypted);
console.log(`decrypted: ${decrypted.toString()}`);
console.log(JSON.stringify(keyStore.getAll()));

let app = express();
let router = express.Router();
let cookieOptions = {
    path: app.path(),
    expires: new Date(Date.now() + 1000*60*60*4)
} as CookieOptions;
var authHelper = new AuthHelper<User>(protector, "auth", cookieOptions, (user: User)  => {
    return user != null && user.userName != null;
});
app.use((req, res, next) => {
    console.log();
    next();
});
app.use(authHelper.getMiddlware());
app.use((req: any, res, next) => {
    console.log('identity:');
    console.log(JSON.stringify(req.identity));
    next();
});
app.use(bodyParser.urlencoded());
app.use('/', router);
//#region router
router.all('/login', (req: any, res, next) => {
    if(req.query.username) {
        console.log('processing request with query');
        let user = new User();
        user.userName = req.query.username;
        req.identity.user = user;
        authHelper.updateAuthCookie(res, req.identity.user);
        return res.redirect('/');
    }
    console.log('processing get request for login page');
    if(req.identity.isAuthenticated) {
        console.log('user authenticated. sending to default page');
        return res.redirect('/');
    }
    else {
        console.log('sending login.html');
        return res.send(fs.readFileSync('./src/server/views/login.html').toString());
    }
});
router.get('/logout', (req: any, res, next) => {
    console.log('logging user out...')
    req.identity.user = null;
    authHelper.updateAuthCookie(res, req.identity.user);
});
router.get('/', (req: any, res, next) => {
    console.log('sending default page...')
    res.send(`user ${JSON.stringify(req.identity.user)}`);
});
//#endregion
app.listen(8081);
