namespace FundManager.Domain

type FundId = int
type TransactionId = int
type PersonId = int

type EntityId = 
    | FundId        of FundId
    | PersonId      of PersonId
    | TransactionId of TransactionId

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

[<CLIMutable>]
type Person =
    { Id        : PersonId
      Name      : string
      StartDate : System.DateTime
      Status    : PersonAttributes.Status }

[<CLIMutable>]
type Fund =
    { Id            : FundId
      YearMonth     : int
      Amount        : decimal
      Contributor   : string 
      Comment       : string } 

[<CLIMutable>]
type Transaction =
    { Id            : TransactionId
      Type          : TransactionAttributes.Type
      Description   : string
      Date          : System.DateTime
      Amount        : decimal
      Currency      : Currency }

type Entity =
    | Fund          of Fund
    | Person        of Person
    | Transaction   of Transaction

