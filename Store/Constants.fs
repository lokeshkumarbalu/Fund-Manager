[<RequireQualifiedAccess>]
module FundManager.Store.Constants

    [<Literal>]
    let DbFileName = "Application.db"

    [<RequireQualifiedAccess>]
    module CsvStructure = 
        [<Literal>]
        let Fund = "CsvStructure\Fund.csv"

        [<Literal>]
        let Person = "CsvStructure\Person.csv"

        [<Literal>]
        let Transaction = "CsvStructure\Transaction.csv"

    [<RequireQualifiedAccess>]
    module CsvFile = 
        [<Literal>]
        let Funds = "Funds.csv"

        [<Literal>]
        let People = "People.csv"

        [<Literal>]
        let Transactions = "Transactions.csv"

    [<RequireQualifiedAccess>]
    module CollectionNames = 
        [<Literal>] 
        let Fund = "funds"

        [<Literal>] 
        let Person = "people"

        [<Literal>] 
        let Transaction = "transactions"

