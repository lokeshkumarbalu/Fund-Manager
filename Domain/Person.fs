[<RequireQualifiedAccess>]
module FundManager.Domain.Person

    type Id = int

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

    type Entity =
        { Id        : Id
          Name      : string
          StartDate : System.DateTime
          Status    : Status }      


