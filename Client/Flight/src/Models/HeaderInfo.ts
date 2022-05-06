export interface HeaderInfo {
    UserId:string,
    TenantId?:string|null,
    Authorization?:string|null,
    RefreshToken?:string|null
}

export enum APIRequestType{
    Get,
    Post,
    Delete,
    Put
};



