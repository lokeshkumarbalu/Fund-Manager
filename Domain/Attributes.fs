namespace FundManager.Domain

[<RequireQualifiedAccess>]
module PersonAttributes =

    type Status = 
        | Active 
        | Inactive
    
        override this.ToString() = 
            match this with
            | Active    -> "Active"
            | Inactive  -> "Inactive"
    
        static member fromString (value: string) = 
            match value with 
            | "Active"      -> Active
            | "Inactive"    -> Inactive
            | _             -> Inactive


[<RequireQualifiedAccess>]
module TransactionAttributes =

    type Type = 
        | Deposit 
        | Withdrawl

        override this.ToString() =
            match this with
            | Deposit   -> "Deposit"
            | Withdrawl -> "Withdrawl"

        static member fromString (value: string) =
            match value.ToLower() with 
            | "deposit"     -> Deposit
            | "deposits"    -> Deposit
            | "withdrawl"   -> Withdrawl
            | "withdrawls"  -> Withdrawl
            | _             -> Withdrawl

