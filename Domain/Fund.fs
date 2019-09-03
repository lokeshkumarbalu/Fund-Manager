[<RequireQualifiedAccess>]
module FundManager.Domain.Fund

    type Id = int

    [<CLIMutable>]
    type Entity =
        { Id            : Id
          YearMonth     : int
          Amount        : decimal
          Contributor   : string 
          Comment       : string } 
