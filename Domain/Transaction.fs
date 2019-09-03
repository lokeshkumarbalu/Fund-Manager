[<RequireQualifiedAccess>]
module FundManager.Domain.Transaction

    type Id = int

    type Type = 
        | Deposit 
        | Withdrawl

        override this.ToString() =
            match this with
            | Deposit   -> "Deposit"
            | Withdrawl -> "Withdrawl"

        static member fromString (value: string) =
            match value with 
            | "Deposits"    -> Deposit
            | "Deposit"     -> Deposit
            | "Withdrawl"   -> Withdrawl
            | "Withdrawls"  -> Withdrawl
            | _             -> Withdrawl

    type Currency = 
    | CHF
    | EUR
    | INR
    | USD

        override this.ToString() =
            match this with 
            | CHF -> "CHF"
            | EUR -> "EUR"
            | INR -> "INR"
            | USD -> "USD"
        
        static member fromString (value: string) =
            match value with
            | "CHF" -> CHF
            | "EUR" -> EUR
            | "INR" -> INR
            | "USD" -> USD
            | _     -> INR


    
    type Entity =
        { Id            : Id
          Type          : Type
          Description   : string
          Date          : System.DateTime
          Amount        : decimal
          Currency      : Currency }




    