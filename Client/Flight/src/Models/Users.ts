
export interface Users {
    Id?:number,
    FirstName?:string,
    LastName?:string,
    EmailId?:string,
    Password?:string,
    AccountStatusId?:number,
    AccountStatus?:string,
    IsSuperAdmin?:boolean,
    CreatedOn?:Date,
    ModifiedOn?:Date
}

export interface AccountStatus {
    Id?:number,
    Status?:string,
    Description?:string
}

export interface UserLoginResponse {
    User?:Users,
    AuthResponse?:AuthResponse
}

export interface AuthResponse {
    Token?:string,
    RefreshToken?:string,
    IsSuccess?:boolean,
    Reason?:string
}

export interface AuthRequest {
    UserName?:string,
    Password?:string
}

export interface RefreshTokenRequest {
    ExpiredToken?:string,
    RefreshToken?:string
}