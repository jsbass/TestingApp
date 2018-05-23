export class Identity<T extends object> {
    private _authFunction: (user: T) => boolean;
    get isAuthenticated(): boolean {
        return this._authFunction(this.user);
    }
    user: T

    constructor(isAuthenticatedFunc: (user: T) => boolean) {
        this._authFunction = isAuthenticatedFunc;
    }
}